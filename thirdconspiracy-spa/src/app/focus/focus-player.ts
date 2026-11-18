import { Component, computed, input, output, signal } from '@angular/core';
import type { RepeatMode } from './focus-music';

export interface FocusOption {
  value: number;
  label: string;
}

let nextId = 0;

/**
 * One Focus Zone player: play/pause, the current selection (options expand on demand),
 * and mute + volume. Optional extras: a repeat toggle, removable options and an add button.
 */
@Component({
  selector: 'app-focus-player',
  templateUrl: './focus-player.html',
  styleUrl: './focus-player.scss',
})
export class FocusPlayer {
  /** Accessible name, e.g. "Noise" or "Music" */
  readonly name = input.required<string>();
  readonly options = input.required<readonly FocusOption[]>();
  readonly selected = input.required<number | null>();
  readonly playing = input.required<boolean>();
  readonly volume = input.required<number>();
  readonly muted = input.required<boolean>();
  readonly error = input<string | null>(null);
  /** Shown in place of the selection when there are no options */
  readonly emptyText = input('Nothing to play');
  /** Shows a repeat toggle in this mode (once there's something to play); hidden when null */
  readonly repeat = input<RepeatMode | null>(null);
  readonly removable = input(false);
  readonly addLabel = input<string | null>(null);

  readonly playToggle = output<void>();
  readonly optionSelect = output<number>();
  readonly volumeChange = output<number>();
  readonly muteToggle = output<void>();
  readonly repeatToggle = output<void>();
  readonly optionRemove = output<FocusOption>();
  readonly add = output<void>();

  protected readonly panelId = `focus-options-${nextId++}`;
  protected readonly expanded = signal(false);
  protected readonly current = computed(() => this.options().find((o) => o.value === this.selected()) ?? null);
  /** With nothing to choose from, the panel stays open so the add button is reachable */
  protected readonly panelOpen = computed(() => this.expanded() || !this.options().length);

  protected onVolume(event: Event): void {
    this.volumeChange.emit((event.target as HTMLInputElement).valueAsNumber);
  }
}
