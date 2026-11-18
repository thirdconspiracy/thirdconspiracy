import { Routes } from '@angular/router';
import {Data} from './data/data';
import {Poker} from './poker/poker';
import {Timer} from './timer/timer';
import {PokerLobby} from './poker/poker.lobby';

export const routes: Routes = [
  {path: '', component: Data},
  {path: 'data', component: Data},
  {path: 'poker', component: PokerLobby},
  {path: 'poker/:tableId', component: Poker},
  {path: 'timer', component: Timer}
];
