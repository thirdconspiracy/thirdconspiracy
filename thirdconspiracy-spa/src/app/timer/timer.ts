import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { Alarm, nextOccurrence, parseClockTime } from './alarm';
import { Countdown } from './countdown';
import { PauseResumeButton } from './pause-resume-button/pause-resume-button';
import { Stopwatch } from './stopwatch';
import { TimeDisplay } from './time-display/time-display';

export type TimerKind = 'countdown' | 'alarm' | 'stopwatch';

// Default invalid/blank to 0
const toInt = (value: string) => Math.max(0, Math.floor(Number(value) || 0));

@Component({
  selector: 'app-timer',
  imports: [DatePipe, PauseResumeButton, TimeDisplay],
  templateUrl: './timer.html',
  styleUrl: './timer.scss'
})
export class Timer {
  protected readonly countdown = inject(Countdown);
  protected readonly alarm = inject(Alarm);
  protected readonly stopwatch = inject(Stopwatch);

  protected readonly active = computed<TimerKind | null>(() => {
    if (this.countdown.status() !== 'idle') return 'countdown';
    if (this.alarm.status() !== 'idle') return 'alarm';
    if (this.stopwatch.status() !== 'idle') return 'stopwatch';
    return null;
  });

  private readonly lastStarted = signal<TimerKind>('countdown');

  // Column highlighted by the vertical ribbon: the running timer, else the last one started.
  protected readonly selected = computed(() => this.active() ?? this.lastStarted());

  protected startCountdown(h: string, m: string, s: string): void {
    const total = toInt(h) * 3600 + toInt(m) * 60 + toInt(s);
    if (total <= 0) return;
    this.switchTo('countdown');
    this.countdown.start(total);
  }

  // Inputs are local 24-hour time. Past times roll over to tomorrow
  protected startAlarm(h: string, m: string, s: string): void {
    const time = parseClockTime(h, m, s);
    if (!time) return;
    this.switchTo('alarm');
    this.alarm.start(nextOccurrence(time));
  }

  protected startStopWatch(): void {
    this.switchTo('stopwatch');
    this.stopwatch.start();
  }

  private switchTo(kind: TimerKind): void {
    this.lastStarted.set(kind);
    this.countdown.stop();
    this.alarm.stop();
    this.stopwatch.stop();
  }
}
