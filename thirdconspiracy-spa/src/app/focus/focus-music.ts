import { Injectable, computed, effect, inject, signal } from '@angular/core';
import { clampVolume, loadSettings, saveSettings, sliderGain } from './focus-settings';
import { FocusTrack, FocusTrackStore } from './focus-track-store';

export type RepeatMode = 'all' | 'one';

const SETTINGS_KEY = 'focus-music';
const MAX_TITLE_LENGTH = 100;

const isMp3 = (file: File): boolean => file.type === 'audio/mpeg' || /\.mp3$/i.test(file.name);

/**
 * Background music from MP3s the user added (saved in this browser by FocusTrackStore).
 * Plays through a single HTMLAudioElement (created on first use) and integrates with the
 * Media Session API so OS media keys/overlays work.
 */
@Injectable({ providedIn: 'root' })
export class FocusMusic {
  private readonly store = inject(FocusTrackStore);
  private audio?: HTMLAudioElement;
  /** The song loaded into the audio element, as an object URL */
  private source?: { id: number; url: string };

  private readonly saved = loadSettings(SETTINGS_KEY, {
    track: null as number | null,
    volume: 0.8,
    repeat: 'all' as RepeatMode,
  });

  readonly tracks = signal<readonly FocusTrack[]>([]);
  readonly playing = signal(false);
  readonly volume = signal(clampVolume(this.saved.volume, 0.8));
  readonly muted = signal(false);
  readonly repeat = signal<RepeatMode>(this.saved.repeat === 'one' ? 'one' : 'all');
  readonly error = signal<string | null>(null);
  private readonly trackId = signal(Number.isInteger(this.saved.track) ? this.saved.track : null);
  readonly track = computed(() => this.tracks().find((t) => t.id === this.trackId()) ?? this.tracks()[0] ?? null);

  readonly ready: Promise<void>;

  constructor() {
    effect(() =>
      saveSettings(SETTINGS_KEY, {
        track: this.track()?.id ?? this.trackId(),
        volume: this.volume(),
        repeat: this.repeat(),
      })
    );
    effect(() => {
      const volume = this.volume();
      const muted = this.muted();
      const loop = this.repeat() === 'one';
      if (this.audio) {
        this.audio.volume = sliderGain(volume);
        this.audio.muted = muted;
        this.audio.loop = loop;
      }
    });
    this.ready = this.store.list().then(
      (tracks) => this.tracks.set(tracks),
      (e: unknown) => this.error.set(`Saved songs are unavailable: ${(e as Error).message}`),
    );
  }

  async toggle(): Promise<void> {
    if (this.playing()) {
      this.audio?.pause();
      return;
    }
    await this.play();
  }

  /** Selects a song; keeps playing (the new song) if music is playing. */
  async selectTrack(id: number): Promise<void> {
    if (id === this.track()?.id || !this.tracks().some((t) => t.id === id)) return;
    const wasPlaying = this.playing();
    this.trackId.set(id);
    this.error.set(null);
    if (wasPlaying) await this.play();
  }

  setVolume(volume: number) {
    this.volume.set(clampVolume(volume, this.volume()));
    if (volume > 0) this.muted.set(false);
  }

  toggleMute(): void {
    this.muted.update((m) => !m);
  }

  toggleRepeat(): void {
    this.repeat.update((r) => (r === 'all' ? 'one' : 'all'));
  }

  /** Saves an MP3 in this browser. Rejects with a user-facing message. */
  async addTrack(title: string, file: File): Promise<FocusTrack> {
    const name = title.trim().slice(0, MAX_TITLE_LENGTH);
    if (!name) throw new Error('Enter a title.');
    if (!isMp3(file)) throw new Error('Choose an MP3 file.');
    await this.ready;
    const track = await this.store.add(name, file);
    const first = !this.tracks().length;
    this.tracks.update((list) => [...list, track]);
    if (first) this.trackId.set(track.id);
    // Ask the browser not to evict saved songs when storage runs low.
    void navigator.storage?.persist?.().catch(() => undefined);
    return track;
  }

  async removeTrack(id: number): Promise<void> {
    await this.ready;
    const current = this.track()?.id === id;
    const next = current ? this.neighbor(1) : null;
    const replacement = next?.id === id ? null : next;
    const keepPlaying = current && this.playing() && !!replacement;
    await this.store.remove(id);
    this.tracks.update((list) => list.filter((t) => t.id !== id));
    if (current) {
      if (!keepPlaying) {
        this.audio?.pause();
        this.onPlayState(false);
      }
      this.trackId.set(replacement?.id ?? null);
      this.error.set(null);
    }
    if (this.source?.id === id) this.unload();
    if (keepPlaying) await this.play();
  }

  private async play(): Promise<void> {
    const track = this.track();
    if (!track) {
      this.playing.set(false);
      return;
    }
    const audio = this.ensureAudio();
    try {
      await this.load(track);
      await audio.play();
      this.error.set(null);
    } catch (e) {
      if ((e as DOMException).name !== 'AbortError') this.reportUnavailable(track);
    }
  }

  private async load(track: FocusTrack): Promise<void> {
    if (this.source?.id === track.id) return;
    const blob = await this.store.audio(track.id);
    if (this.track()?.id !== track.id) throw new DOMException('Superseded', 'AbortError');
    if (this.source) URL.revokeObjectURL(this.source.url);
    this.source = { id: track.id, url: URL.createObjectURL(blob) };
    this.audio!.src = this.source.url;
  }

  private unload(): void {
    if (!this.source) return;
    URL.revokeObjectURL(this.source.url);
    this.source = undefined;
  }

  private neighbor(delta: number): FocusTrack | null {
    const list = this.tracks();
    if (!list.length) return null;
    const index = list.findIndex((t) => t.id === this.track()?.id);
    return list[(index + delta + list.length) % list.length];
  }

  private async advance(): Promise<void> {
    const next = this.neighbor(1);
    if (next) this.trackId.set(next.id);
    await this.play();
  }

  private ensureAudio(): HTMLAudioElement {
    if (this.audio) return this.audio;
    const audio = new Audio();
    audio.loop = this.repeat() === 'one';
    audio.volume = sliderGain(this.volume());
    audio.muted = this.muted();
    audio.addEventListener('play', () => this.onPlayState(true));
    // A song ending fires 'pause' then 'ended'; stay "playing" while moving to the next one.
    audio.addEventListener('pause', () => {
      if (!audio.ended) this.onPlayState(false);
    });
    audio.addEventListener('ended', () => void this.advance());
    audio.addEventListener('error', () => {
      const track = this.track();
      if (track) this.reportUnavailable(track);
    });
    this.audio = audio;
    this.registerMediaSession();
    return audio;
  }

  private onPlayState(playing: boolean): void {
    this.playing.set(playing);
    if (!('mediaSession' in navigator)) return;
    navigator.mediaSession.playbackState = playing ? 'playing' : 'paused';
    const track = this.track();
    if (playing && track) {
      navigator.mediaSession.metadata = new MediaMetadata({ title: track.title, album: 'Focus Zone' });
    }
  }

  private registerMediaSession(): void {
    if(!('mediaSession' in navigator)) return;
    const session = navigator.mediaSession;
    const step = (delta: number) => () => {
      const track = this.neighbor(delta);
      if (track) void this.selectTrack(track.id);
    };
    session.setActionHandler('play', () => void this.play());
    session.setActionHandler('pause', () => this.audio?.pause());
    session.setActionHandler('nexttrack', step(1));
    session.setActionHandler('previoustrack', step(-1));
  }

  private reportUnavailable(track: FocusTrack): void {
    this.playing.set(false);
    this.error.set(
      `Couldn't play \u201c${track.title}\u201d. Try removing it and adding the MP3 again.`,
    );
  }
}
