import {Player} from './player.model';

export interface PokerTable {
  id: string;
  show: boolean;
  players: { [connectionId: string]: Player };
}
