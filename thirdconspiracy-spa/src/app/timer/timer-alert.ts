import { Injectable, inject } from '@angular/core';
import { Title } from '@angular/platform-browser';

const RING_INTERVAL_MS= 2000;
const STRIKES_PER_RING = 16;
const STRIKE_SPACINGS_S = 0.075;
const BELL_FUNDAMENTALS_HZ = [1400, 1520];

const BELL_PARTIALS: ReadonlyArray<readonly [number, number]> = [
  [1, 1],
  [2.76, 0.5],
  [5.4, 0.25],
  [8.93, 0.12]
];

@Injectable({ providedIn: 'root' })
export class TimerAlert {
  private readonly title = inject(Title);
  private ctx?: AudioContext;
  private output?: GainNode;
  private ringId?: ReturnType<typeof setInterval>;
  private originalTitle?: string;

  prime(): void {
    if (typeof AudioContext === 'undefined') return;
    this.ctx ??= new AudioContext();
    void this.ctx.resume();
  }

  start(message = "⏰ Time's up!"): void {
    this.stop();
    this.originalTitle = this.title.getTitle();
    this.title.setTitle(message);

    if (!this.ctx) return;
    void this.ctx.resume();
    this.output = this.ctx.createGain();
    this.output.gain.value = 0.25;
    this.output.connect(this.ctx.destination);
    this.ring();
    this.ringId = setInterval(() => this.ring(), RING_INTERVAL_MS);
  }

  stop(): void {
    clearInterval(this.ringId);
    this.ringId = undefined;

    if (this.ctx && this.output) {
      const out = this.output;
      out.gain.setTargetAtTime(0, this.ctx.currentTime, 0.02);
      setTimeout(() => out.disconnect(), 150);
      this.output = undefined;
    }

    if (this.originalTitle !== undefined) {
      this.title.setTitle(this.originalTitle);
      this.originalTitle = undefined;
    }
  }

  private ring(): void {
    const ctx = this.ctx;
    const out = this.output;
    if(!ctx || !out) return;

    const start = ctx.currentTime + 0.05;
    for (let i = 0; i < STRIKES_PER_RING; i++) {
      this.strike(ctx, out, start + i * STRIKE_SPACINGS_S, BELL_FUNDAMENTALS_HZ[i % 2]);
    }
  }

  private strike(ctx: AudioContext, out: AudioNode, at: number, fundamental: number): void {
    for (const [ratio, level] of BELL_PARTIALS) {
      const decay = 1.2 / ratio;
      const osc = ctx.createOscillator();
      const env = ctx.createGain();
      osc.frequency.value = fundamental * ratio;
      env.gain.setValueAtTime(0, at);
      env.gain.linearRampToValueAtTime(level, at + 0.002);
      env.gain.exponentialRampToValueAtTime(0.0001, at + decay);
      osc.connect(env).connect(out);
      osc.start(at);
      osc.stop(at + decay);
    }
  }
}
