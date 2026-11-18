import { Component, Input, Output, EventEmitter} from '@angular/core';
import { PokerTable} from '../models/poker-table.model';
import {Player} from '../models/player.model';

@Component({
  selector: 'app-poker-actions',
  standalone: true,
  templateUrl: './poker.actions.html',
  styleUrl: './poker.actions.css',
})
export class PokerActions {
  @Input() table?: PokerTable;
  @Input() currentPlayer?: Player;

  @Output() dealClicked = new EventEmitter<void>();
  @Output() showToggled = new EventEmitter<void>();
  @Output() isPlayingToggled = new EventEmitter<void>();

  onDealClick() {
    this.dealClicked.emit();
  }

  onShowToggle() {
    this.showToggled.emit();
  }

  onIsPlayingToggle() {
    this.isPlayingToggled.emit();
  }
}
