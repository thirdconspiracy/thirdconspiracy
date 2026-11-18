import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'data' },
  { path: 'data', title: 'Data',
    loadComponent: () => import('./data/data').then((m) => m.Data) },
  { path: 'timer', title: 'Timers',
    loadComponent: () => import('./timer/timer').then((m) => m.Timer) },
  { path: 'poker', title: 'Data',
    loadComponent: () => import('./poker/poker').then((m) => m.Poker), },
  { path: 'poker/:tableId', title: 'Pointing Poker',
    loadComponent: () => import('./poker/poker').then((m) => m.Poker), },
  { path: '**', redirectTo: '' },
];
