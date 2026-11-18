import {Estimate} from './estimate.enum';

export interface Player {
  connectionId: string;
  name: string;
  groupName: string;
  estimate: Estimate;
  isPlaying: boolean;
}
