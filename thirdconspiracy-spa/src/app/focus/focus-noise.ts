import { Injectable, computed, effect, signal } from '@angular/core';
import { clampVolume, loadSettings, saveSettings, sliderGain } from './focus-settings';

export const NOISE_TYPES = [
  { value: 0, label: 'White Noise' },
  { value: 1, label: 'Pink Noise' },
  { value: 2, label: 'Brown Noise' },
] as const;
export type NoiseType = (typeof NOISE_TYPES)[number]['value'];

const SETTINGS_KEY = 'focus-noise';
const WORKLET_URL = '/noise-processor.js';
const FADE_S = 0.05;
const SUSPEND_AFTER_MS = 250;

/**
 * Background noise via AudioWorklet. Audio graph is built on the first play
 * (a user gesture, so autoplay rules allow it).  When paused, the context is
 * suspended so the generator uses no CPU.
 */
@Injectable({ providedIn: 'root' })
export class FocusNoise {
  private ctx?: AudioContext;
  private gain?: GainNode;
  private node?: AudioWorkletNode;
  private suspendTimer?: ReturnType<typeof setTimeout>;

  private readonly saved = loadSettings(SETTINGS_KEY, { type: 0 as NoiseType, volume: 0.5 });

  readonly playing = signal(false);
  readonly type = signal<NoiseType>(
    NOISE_TYPES.some((t) => t.value === this.saved.type) ? this.saved.type : 0,
  );
  readonly volume = signal(clampVolume(this.saved.volume));
  readonly muted = signal(false);
  readonly error = signal<string | null>(null);
  private readonly level = computed(() =>
    this.playing() && !this.muted() ? sliderGain(this.volume()) : 0,
  );

  constructor() {
    effect(() => saveSettings(SETTINGS_KEY, { type: this.type(), volume: this.volume() }));
    effect(() => this.rampTo(this.level()));
    effect(() => {
      const type = this.type();
      if (this.ctx)
        this.node?.parameters.get('noiseType')?.setValueAtTime(type, this.ctx.currentTime);
    });
  }

  async toggle(): Promise<void> {
    if (this.playing()) {
      this.playing.set(false);
      clearTimeout(this.suspendTimer);
      // Let the fade-out finish before suspending.
      this.suspendTimer = setTimeout(
        () => !this.playing() && this.ctx?.suspend(),
        SUSPEND_AFTER_MS,
      );
      return;
    }
    try {
      await this.ensureGraph();
      clearTimeout(this.suspendTimer);
      await this.ctx!.resume();
      this.error.set(null);
      this.playing.set(true);
    } catch (e) {
      this.error.set(`Noise unavailable: ${(e as Error).message}`);
    }
  }

  setType(type: NoiseType): void {
    this.type.set(type);
  }

  setVolume(volume: number): void {
    this.volume.set(clampVolume(volume, this.volume()));
    if (volume > 0) this.muted.set(false);
  }

  toggleMute(): void {
    this.muted.update((m) => !m);
  }

  private async ensureGraph(): Promise<void> {
    if (this.node) return;
    const ctx = (this.ctx ??= new AudioContext());
    await ctx.audioWorklet.addModule(WORKLET_URL);
    this.gain = new GainNode(ctx, { gain: 0 });
    this.node = new AudioWorkletNode(ctx, 'noise-processor', { parameterData: { noiseType: this.type() } });
    this.node.connect(this.gain).connect(ctx.destination);
  }

  /** Smoothly moves the gain to avoid clicks */
  private rampTo(level: number): void {
    if (!this.ctx || !this.gain) return;
    this.gain.gain.setTargetAtTime(level, this.ctx.currentTime, FADE_S);
  }
}
