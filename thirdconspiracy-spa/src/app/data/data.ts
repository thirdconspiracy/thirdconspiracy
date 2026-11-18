import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { DATA_ACTIONS, DataAction } from './data-actions';

@Component({
  selector: 'app-data',
  templateUrl: './data.html',
  styleUrl: './data.scss',
})
export class Data {
  protected readonly actions = DATA_ACTIONS;
  protected readonly action = signal<DataAction>(DATA_ACTIONS[0]);
  protected readonly input = signal(DATA_ACTIONS[0].example);
  private readonly result = computed(() => {
    try {
      return { output: this.action().transform(this.input()), error: null };
    } catch (e) {
      return { output: '', error: e instanceof Error ? e.message : String(e) };
    }
  });
  protected readonly output = computed(() => this.result().output);
  protected readonly error = computed(() => this.result().error);
  protected readonly copied = signal(false);

  private copiedTimer?: ReturnType<typeof setTimeout>;

  constructor() {
    inject(DestroyRef).onDestroy(() => clearTimeout(this.copiedTimer));
  }

  protected select(action: DataAction): void {
    this.action.set(action);
    this.input.set(action.example);
  }

  protected async copy(): Promise<void> {
    const output = this.output();
    if (!output) return;
    try {
      await navigator.clipboard.writeText(output);
    } catch {
      // Clipboard access denied (permissions, unfocused page or non-HTTPS); leave "Copy" as is.
      return;
    }
    this.copied.set(true);
    clearTimeout(this.copiedTimer);
    this.copiedTimer = setTimeout(() => this.copied.set(false), 1500);
  }
}
