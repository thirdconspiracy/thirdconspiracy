import { Component, ElementRef, afterNextRender, inject, input, output, signal, viewChild } from '@angular/core';
import { FocusMusic } from './focus-music';
import { FocusTrack} from './focus-track-store';

/** Confirms deleting a saved song from this browser */
@Component({
  selector: 'app-focus-remove-dialog',
  templateUrl: './focus-remove-dialog.html',
})
export class FocusRemoveDialog {
  readonly track = input.required<FocusTrack>();
  readonly closed = output<void>();

  private readonly music = inject(FocusMusic);
  private readonly dialog = viewChild.required<ElementRef<HTMLDialogElement>>('dialog');

  protected readonly removing = signal(false);
  protected readonly error = signal<string | null>(null);

  constructor() {
    afterNextRender(() => this.dialog().nativeElement.showModal())
  }

  protected async confirm(): Promise<void> {
    if (this.removing()) return;
    this.removing.set(true);
    this.error.set(null);
    try {
      await this.music.removeTrack(this.track().id);
      this.close();
    } catch (e) {
      this.error.set(`Couldn't remove the song: ${(e as Error).message}`);
    } finally {
      this.removing.set(false);
    }
  }

  protected close(): void {
    this.dialog().nativeElement.close();
  }
}
