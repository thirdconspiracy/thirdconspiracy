import { Component, Input, Output, EventEmitter} from '@angular/core';
import {JsonPipe} from '@angular/common';
import {Player} from '../models/player.model';
import {PokerTable} from '../models/poker-table.model';
import {getEstimatedDisplay} from '../models/estimate-display.util';

@Component({
  selector: 'app-poker-table',
  standalone: true,
  imports: [JsonPipe],
  templateUrl: './poker-table.component.html',
  styleUrl: './poker-table.component.css',
})
export class PokerTableComponent {
  @Input() table?: PokerTable;
  @Output() makeObserverClicked = new EventEmitter<string>();
  @Output() nudgePlayerClicked = new EventEmitter<string>();

  public showDebug: boolean = false;

  get playerList(): Player[] {
    return this.table ? Object.values(this.table.players) : [];
  }

  protected readonly getEstimateDisplay = getEstimatedDisplay;

  makeObserver(connectionId: string){
    this.makeObserverClicked.emit(connectionId);
  }

  nudgePlayer(connectionId: string){
    this.nudgePlayerClicked.emit(connectionId);
  }

  toggleDebug() {
    this.showDebug = !this.showDebug;
  }

}
