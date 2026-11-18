import { Estimate} from './estimate.enum';

export function getEstimatedDisplay(estimate: Estimate): string {
  switch (estimate) {
    case Estimate.None: return 'None';
    case Estimate.Unknown: return 'Unknown';
    case Estimate.Coffee: return 'Coffee';
    case Estimate.Infinite: return '∞';
    default: return estimate.toString();
  }
}
