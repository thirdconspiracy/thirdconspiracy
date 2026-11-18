import { Component, ElementRef, afterNextRender, inject, output, signal, viewChild } from '@angular/core';
import { FocusMusic } from './focus-music';

/** Suggested title from a file name, e.g. "River_Flows_in_You.mp3" -> "River Flows in You" */
export const titleFromFileName = (name: string): string =>
  name
    .replace(/\.[^.]*$/, '')
    .replace(/_+/g, ' ')
    .trim();

/** Modal for adding an MP3 (saved in this browser) to Focus Zone music */
@Component({
  selector: 'app-focus-add-dialog',
  templateUrl: './focus-add-dialog.html',
})
export class FocusAddDialog {
  readonly closed = output<void>();

  private readonly music = inject(FocusMusic);
  private readonly dialog = viewChild.required<ElementRef<HTMLDialogElement>>('dialog');

  protected readonly file = signal<File | null>(null);
  protected readonly title = signal('');
  protected readonly saving = signal(false);
  protected readonly error = signal<string | null>(null);
  /** Title suggested from the last picked file -- replace on a new pick unless the user edited it */
  private suggested = '';

  constructor() {
    afterNextRender(() => this.dialog().nativeElement.showModal());
  }

  protected onFile(input: HTMLInputElement): void {
    const file = input.files?.[0] ?? null;
    this.file.set(file);
    this.error.set(null);
    if (file && (!this.title().trim() || this.title() === this.suggested)) {
      this.suggested = titleFromFileName(file.name);
      this.title.set(this.suggested);
    }
  }

  protected async submit(event: Event): Promise<void> {
    event.preventDefault();
    const file = this.file();
    if (!file || this.saving()) return;
    this.saving.set(true);
    this.error.set(null);
    try {
      await this.music.addTrack(this.title(), file);
      this.close();
    } catch (e) {
      this.error.set((e as Error).message || "Couldn't save the song.");
    } finally {
      this.saving.set(false);
    }
  }

  protected close(): void {
    this.dialog().nativeElement.close();
  }
}
