export const Estimate = {
  None: 0,
  One: 1,
  Two: 2,
  Three: 3,
  Five: 5,
  Eight: 8,
  Thirteen: 13,
  Twenty: 20,
  Unknown: 97,
  Coffee: 98,
  Infinite: 99,
} as const;
export type Estimate = (typeof Estimate)[keyof typeof Estimate];

export interface Player {
  connectionId: string;
  name: string;
  groupName: string;
  estimate: Estimate;
  isPlaying: boolean;
}

export interface PokerTable {
  id: string;
  show: boolean;
  players: Readonly<Record<string, Player>>;
}

export const HAND: readonly { estimate: Estimate; label: string }[] = [
  { estimate: Estimate.Coffee, label: 'Coffee' },
  { estimate: Estimate.One, label: '1 Point' },
  { estimate: Estimate.Two, label: '2 Points' },
  { estimate: Estimate.Three, label: '3 Points' },
  { estimate: Estimate.Five, label: '5 Points' },
  { estimate: Estimate.Eight, label: '8 Points' },
  { estimate: Estimate.Thirteen, label: '13 Points' },
  { estimate: Estimate.Twenty, label: '20 Points' },
  { estimate: Estimate.Unknown, label: '? Points' },
  { estimate: Estimate.Infinite, label: '∞ Points' },
];

export function estimateDisplay(estimate: Estimate): string {
  switch (estimate) {
    case Estimate.None:
      return 'None';
    case Estimate.Unknown:
      return 'Unknown';
    case Estimate.Coffee:
      return 'Coffee';
    case Estimate.Infinite:
      return '∞';
    default:
      return String(estimate);
  }
}
