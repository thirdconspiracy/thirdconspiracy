import { DOCUMENT, DestroyRef, Injectable, computed, effect, inject, signal } from '@angular/core';
import { Alarm } from './alarm';
import { Countdown } from './countdown';

const STEPS = 100;
// Light-theme --color-primary; used when the CSS variable is unavailable.
const DEFAULT_COLOR = '#006399';

// SVG data URL of pie percentage as clockwise shrinking wedge
export function pieIcon(elapsed: number, color = DEFAULT_COLOR): string {
  const remaining = 1 - Math.min(1, Math.max(0, elapsed));
  let wedge = '';
  if (remaining >= 1) {
    wedge = `<circle cx="16" cy="16" r="14" fill="${color}"/>`;
  } else if (remaining > 0) {
    const angle = 2 * Math.PI * (1 - remaining);
    const x = (16 + 14 * Math.sin(angle)).toFixed(2);
    const y = (16 - 14 * Math.cos(angle)).toFixed(2);
    const large = remaining > 0.5 ? 1 : 0;
    wedge = `<path d="M16 16L${x} ${y}A14 14 0 ${large} 1 16 2Z" fill="${color}"/>`;
  }
  const svg =
    `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 32 32">` +
    `<circle cx="16" cy="16" r="15" fill="#fff" stroke="${color}" stroke-width="2"/>${wedge}</svg>`;
  return `data:image/svg+xml,${encodeURIComponent(svg)}`
}

// Replaces the favicon with a pie-chart countdown while a countdown or alarm is active.
@Injectable({ providedIn: 'root' })
export class FaviconPie {
  private readonly document = inject(DOCUMENT);
  private readonly countdown = inject(Countdown);
  private readonly alarm = inject(Alarm);
  private link?: HTMLLinkElement;
  private originalHref?: string;
  private originalType = '';

  private readonly step = computed(() => {
    const timer =
      this.countdown.status() !== 'idle' ? this.countdown : this.alarm.status() != 'idle' ? this.alarm : null;
    return timer ? Math.floor(timer.progress() * STEPS) : null;
  });

  private readonly color = signal(this.readColor());

  constructor() {
    const root = this.document.documentElement;
    const observer = new MutationObserver(() => this.color.set(this.readColor()));
    observer.observe(root, { attributes: true, attributeFilter: ['data-theme'] });
    inject(DestroyRef).onDestroy(() => observer.disconnect());
    effect(() => this.render(this.step(), this.color()));
  }

  private readColor(): string {
    const value = getComputedStyle(this.document.documentElement).getPropertyValue('--color-primary').trim();
    return value || DEFAULT_COLOR;
  }

  private render(step: number | null, color: string): void {
    const link = this.iconLink();
    if (step === null) {
      link.href = this.originalHref!;
      link.type = this.originalType;
      return;
    }
    link.type = 'image/svg+xml';
    link.href = pieIcon(step / STEPS, color);
  }

  private iconLink(): HTMLLinkElement {
    if (!this.link) {
      let link = this.document.querySelector<HTMLLinkElement>('link[rel~="icon"]');
      if (!link) {
        link = this.document.createElement('link');
        link.rel = 'icon';
        this.document.head.appendChild(link);
      }
      this.link = link;
      this.originalHref = link.getAttribute('href') ?? 'favicon.ico';
      this.originalType = link.type;
    }
    return this.link;
  }
}
