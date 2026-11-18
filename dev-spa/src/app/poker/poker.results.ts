import {Component, Input} from '@angular/core';
import {PokerTable} from '../models/poker-table.model';
import {Estimate} from '../models/estimate.enum';
import {getEstimatedDisplay} from '../models/estimate-display.util';

@Component({
  selector: 'app-poker-results',
  standalone: true,
  templateUrl: './poker.results.html',
  styleUrl: './poker.results.css'
})
export class PokerResults {
  @Input() table?: PokerTable;

  get estimateCounts(): { estimate: Estimate, count: number }[] {
    if (!this.table) return [];
    const counts: Record<number, number> = {};
    Object.values(this.table.players).forEach(player => {
      if (player.isPlaying) {
        counts[player.estimate] = (counts[player.estimate] || 0) + 1;
      }
    });
    return Object
      .entries(counts)
      .map(([estimate, count]) => ({estimate: Number(estimate), count}));
  }

  get totalVotes(): number {
    if (!this.table) return 0;
    return Object.values(this.table.players).filter(player => player.isPlaying).length;
  }

  get consensusEstimate(): Estimate | null {
    const counts = this.estimateCounts;
    if (counts.length === 1 && counts[0].count === this.totalVotes && this.totalVotes > 0) {
      return counts[0].estimate;
    }
    return null;
  }

  protected readonly getEstimateDisplay = getEstimatedDisplay;

}
