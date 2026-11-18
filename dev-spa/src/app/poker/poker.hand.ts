import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Estimate} from '../models/estimate.enum';

@Component({
  selector: 'app-poker-hand',
  standalone: true,
  templateUrl: './poker.hand.html',
  styleUrl: './poker.hand.css',
})
export class PokerHand {
  @Input() isNudged: boolean = false;
  @Output() estimateSelected = new EventEmitter<Estimate>();

  public Estimate = Estimate;

  onEstimateClick(estimate: Estimate) {
    this.estimateSelected.emit(estimate);
  }
}
