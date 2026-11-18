import { Component, computed, inject, signal } from '@angular/core';
import { FocusAddDialog } from './focus-add-dialog';
import { FocusMusic } from './focus-music';
import { FocusNoise, NOISE_TYPES } from './focus-noise';
import { FocusOption, FocusPlayer } from './focus-player';
import { FocusRemoveDialog } from './focus-remove-dialog';
import { FocusTrack } from './focus-track-store';

@Component({
  selector: 'app-focus-menu',
  imports: [FocusPlayer, FocusRemoveDialog, FocusAddDialog],
  templateUrl: './focus-menu.html',
  styleUrl: './focus-menu.scss',
})
export class FocusMenu {
  protected readonly noise = inject(FocusNoise);
  protected readonly music = inject(FocusMusic);

  protected readonly noiseTypes = NOISE_TYPES;
  protected readonly trackOptions = computed(() =>
    this.music.tracks().map((t) => ({ value: t.id, label: t.title })),
  );
  protected readonly anyPlaying = computed(() => this.noise.playing() || this.music.playing());

  protected readonly adding = signal(false);
  protected readonly removing = signal<FocusTrack | null>(null);

  protected confirmRemove(option: FocusOption): void {
    this.removing.set({ id: option.value, title: option.label });
  }
}
