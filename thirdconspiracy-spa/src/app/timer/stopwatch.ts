import { Injectable, computed, signal } from '@angular/core';

export type StopwatchStatus = 'idle' | 'running' | 'paused';

const TICKS_MS = 250;

//Stopwatch counting up from 0 (monotonic clock to avoid drift)
@Injectable({ providedIn: 'root'})
export class Stopwatch {
  private readonly _status = signal<StopwatchStatus>('idle');
  private readonly elapsedMs = signal(0);
  // performance.now() value where elapsed = 0 while running
  private startedAt = 0;
  private tickId?: ReturnType<typeof setInterval>;

  readonly status = this._status.asReadonly();
  // Whole elapsed seconds
  readonly elapsed = computed(() => Math.floor(this.elapsedMs() / 1000));

  // Always restart at 0
  start(): void {
    this.elapsedMs.set(0);
    this.run();
  }

  pause(): void {
    if (this.status() !== 'running') return;
    this.clearTimer();
    this.elapsedMs.set(performance.now() - this.startedAt);
    this._status.set('paused');
  }

  resume(): void {
    if(this._status() !== 'paused') return;
    this.run();
  }

  toggle(): void {
    if (this._status() === 'running') this.pause();
    else this.resume();
  }

  stop(): void {
    this.clearTimer();
    this.elapsedMs.set(0);
    this._status.set('idle');
  }

  private run(): void {
    this.clearTimer();
    this.startedAt = performance.now() - this.elapsedMs();
    this._status.set('running');
    this.tickId = setInterval(() => this.elapsedMs.set(performance.now() - this.startedAt), TICKS_MS);
  }

  private clearTimer(): void {
    clearInterval(this.tickId);
    this.tickId = undefined;
  }
}
