import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-pause-resume-button',
  template: `
    <button
      type="button"
      class="btn-soft"
      [class.btn-icon-play]="paused()"
      [class.btn-icon-pause]="!paused()"
      (click)="toggle.emit()"
    >
      {{ paused() ? 'Resume' : 'Pause' }}
    </button>
  `,
  styles: `
   :host {
     display: contents;
   }
  `
})
export class PauseResumeButton {
  readonly paused = input.required<boolean>();
  readonly toggle = output<void>();
}
