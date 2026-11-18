import { TimerAlert } from './timer-alert';
import { computed, inject, Injectable, signal } from '@angular/core';

export type CountdownStatus = 'idle' | 'running' | 'paused' | 'done';

const TICK_MS = 250;

@Injectable({ providedIn: 'root' })
export class Countdown {
  private readonly alert = inject(TimerAlert);

  private readonly _status = signal<CountdownStatus>('idle');
  private readonly remainingMs = signal(0);
  private readonly totalMs = signal(0);
  private endAt = 0;
  private tickId?: ReturnType<typeof setInterval>;
  private doneId?: ReturnType<typeof setTimeout>;

  readonly status = this._status.asReadonly();
  // Whole seconds left, rounded up, so the display reaches 0 only when done.
  readonly remaining = computed(() => Math.ceil(this.remainingMs() / 1000));
  // Fraction elapsed, 0..1; 1 when done, 0 when idel.
  readonly progress = computed(() => {
    if (this._status() === 'done') return 1;
    const total = this.totalMs();
    return total > 0 ? Math.min(1, Math.max(0, 1 - this.remainingMs() / total)) : 0;
  });

  start (totalSeconds: number): void {
    if (!(totalSeconds > 0)) return;
    this.clearTimers();
    this.alert.stop();
    this.alert.prime();
    this.remainingMs.set(totalSeconds * 1000);
    this.totalMs.set(totalSeconds * 1000);
    this.run();
  }

  pause(): void {
    if (this._status() !== 'running') return;
    this.clearTimers();
    this.remainingMs.set(Math.max(0, this.endAt - performance.now()));
    this._status.set('paused');
  }

  resume(): void {
    if (this._status() !== 'paused') return;
    this.alert.prime();
    this.run();
  }

  toggle(): void {
    if (this._status() === 'running') this.pause();
    else this.resume();
  }

  stop() : void {
    this.clearTimers();
    this.alert.stop();
    this.remainingMs.set(0);
    this.totalMs.set(0);
    this._status.set('idle');
  }

  private run(): void {
    const ms = this.remainingMs();
    this.endAt = performance.now() + ms;
    this._status.set('running');
    this.tickId = setInterval(() => this.remainingMs.set(Math.max(0, this.endAt - performance.now())), TICK_MS);
    this.doneId = setTimeout(() => this.finish(), ms);
  }

  private finish(): void {
    this.clearTimers();
    this.remainingMs.set(0);
    this._status.set('done');
    this.alert.start();
  }

  private clearTimers(): void {
    clearInterval(this.tickId);
    clearTimeout(this.doneId);
    this.tickId = this.doneId = undefined;
  }
}
