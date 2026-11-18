import { Injectable, computed, inject, signal } from '@angular/core';
import { TimerAlert} from './timer-alert';

export type AlarmStatus = 'idle' | 'running' | 'done';

export interface ClockTime {
  h: number;
  m: number;
  s: number;
}

const TICK_MS = 250;

export function parseClockTime(h: string, m: string, s: string): ClockTime | null {
  if([h, m, s].every((v) => v.trim() === '')) return null;
  const [hh, mm, ss] = [h, m, s].map((v) => (v.trim() === '' ? 0 : Number(v)));
  const valid = (n: number, max: number) => Number.isInteger(n) && n >= 0 && n <= max;
  return valid(hh, 23) && valid(mm, 59) && valid(ss, 59) ? { h: hh, m: mm, s: ss } : null;
}

export function nextOccurrence({ h, m, s }: ClockTime, now= new Date()): Date {
  const at = new Date(now);
  at.setHours(h, m, s, 0);
  if (at <= now) at.setDate(at.getDate() + 1);
  return at;
}

@Injectable({ providedIn: 'root'})
export class Alarm {
  private readonly alert = inject(TimerAlert);

  private readonly _status = signal<AlarmStatus>('idle');
  private readonly _until = signal<Date | null>(null);
  private readonly startedAt = signal(0);
  private readonly now = signal(Date.now());
  private tickId?: ReturnType<typeof setInterval>;
  private doneId?: ReturnType<typeof setInterval>;

  readonly status = this._status.asReadonly();
  readonly until = this._until.asReadonly();
  readonly remaining = computed(() => {
    const until = this._until();
    return until ? Math.max(0, Math.ceil((until.getTime() - this.now()) / 1000)) : 0;
  });

  readonly progress = computed(() => {
    if (this._status() === 'done') return 1;
    const until = this._until();
    if (!until) return 0;
    const total = until.getTime() - this.startedAt();
    return total > 0 ? Math.min(1, Math.max(0, (this.now() - this.startedAt()) / total)) : 1;
  });

  start(until: Date): void {
    this.clearTimers();
    this.alert.stop();
    this.alert.prime();
    this._until.set(until);
    this.startedAt.set(Date.now());
    this.now.set(Date.now());
    this._status.set('running');
    this.tickId = setInterval(() => this.tick(), TICK_MS);
    this.doneId = setTimeout(() => this.finish(), Math.max(0, until.getTime() - Date.now()));
  }

  stop(): void {
    this.clearTimers();
    this.alert.stop();
    this._until.set(null);
    this._status.set('idle');
  }

  private tick(): void {
    this.now.set(Date.now());
    if (this.remaining() === 0) this.finish();
  }

  private finish(): void {
    if (this._status() !== 'running') return;
    this.clearTimers();
    this.now.set(Date.now());
    this._status.set('done');
    this.alert.start();
  }

  private clearTimers(): void {
    clearInterval(this.tickId);
    clearTimeout(this.doneId);
    this.tickId = this.doneId = undefined;
  }
}
