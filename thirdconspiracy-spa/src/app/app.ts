import { Component, effect, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { FocusMenu } from './focus/focus-menu';
import { FaviconPie } from './timer/favicon-pie';
import { UserMenu} from './user/user-menu';

export type Theme = 'light' | 'jewel' | 'dark';

@Component({
  imports: [RouterOutlet, RouterLink, RouterLinkActive, FocusMenu, UserMenu],
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly theme = signal<Theme>('light');

  constructor() {
    inject(FaviconPie);
    effect(() => document.documentElement.setAttribute('data-theme', this.theme()));
  }
}
