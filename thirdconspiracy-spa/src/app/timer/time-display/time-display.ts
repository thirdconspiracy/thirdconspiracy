import { Component, computed, input } from '@angular/core';
import { durationFormat, formatDuration } from '../format-duration';

@Component({
  selector: 'app-time-display',
  template: `{{ text() }}`,
  styleUrl: './time-display.scss',
  host: {
    role: 'timer',
    '[class.paused]': 'paused()',
    '[attr.data-format]': 'format()'
  }
})
export class TimeDisplay {
  readonly seconds = input.required<number>();
  readonly paused = input(false);

  protected readonly text = computed(() => formatDuration(this.seconds()));
  protected readonly format = computed(() => durationFormat(this.seconds()));
}
