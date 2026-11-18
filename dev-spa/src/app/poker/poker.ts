import {Component, OnDestroy, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {Login} from '../login/login';
import {PokerActions} from './poker.actions';
import {PokerHand} from './poker.hand';
import {PokerTableComponent} from './poker-table.component';
import {PokerResults} from './poker.results';
import {PokerTable} from '../models/poker-table.model';
import {Player} from '../models/player.model';
import {PokerSignalr} from './poker.signalr';
import {ActivatedRoute} from '@angular/router';
import {UserService} from '../login/user.service';
import {User} from '../models/user.model';
import {Estimate} from '../models/estimate.enum';

@Component({
  selector: 'app-poker',
  standalone: true,
  imports: [
    Login,
    PokerActions,
    PokerHand,
    PokerTableComponent,
    PokerResults,
    CommonModule
  ],
  templateUrl: './poker.html',
  styleUrl: './poker.css',
})
export class Poker {
  public table?: PokerTable;
  public currentPlayer?: Player;
  public currentUser?: User | null;
  public tableId!: string;
  public isNudged: boolean = false;

  constructor(
    private pokerSignalR: PokerSignalr,
    private route: ActivatedRoute,
    private userService: UserService
  ) {
  }

  ngOnInit() {
    this.userService.user$.subscribe(user => {
      if (!user && this.currentPlayer) {
        this.pokerSignalR.disconnect();
      }

      this.currentUser = user;

      if (user && this.tableId) {
        this.initializePokerConnection(user.name);
      }
    });

    this.pokerSignalR.table$.subscribe(table => {
      this.table = table;
    });

    this.pokerSignalR.currentPlayer$.subscribe(player => {
      this.currentPlayer = player;
    });

    this.pokerSignalR.nudge$.subscribe(() => {
      this.handleNudge();
    });

    this.route.paramMap.subscribe(params => {
      const id = params.get('tableId');
      if (id){
        this.tableId = id;

        const currentUser = this.userService.getCurrentUser();
        if (currentUser){
          this.initializePokerConnection(currentUser.name);
        }
      }
    });
  }

  private async initializePokerConnection(playerName: string) {
    if (this.tableId && playerName) {
      await this.pokerSignalR.initConnection(this.tableId, playerName)
    }
  }

  ngOnDestroy() {
    this.pokerSignalR.disconnect();
  }

  notifyDeal() {
    this.pokerSignalR.notifyDeal();
  }

  toggleShow() {
    const show = !this.table?.show;
    this.pokerSignalR.notifyShow(show);
  }

  toggleIsPlaying() {
    const isPlaying = !this.currentPlayer?.isPlaying;
    this.pokerSignalR.notifyIsPlaying(isPlaying);
  }

  notifyEstimate(estimate: Estimate) {
    this.pokerSignalR.notifyEstimate(estimate);
  }

  makeObserver(connectionId: string) {
    this.pokerSignalR.makeObserver(connectionId);
  }

  nudgePlayer(connectionId: string) {
    this.pokerSignalR.notifyNudge(connectionId);
  }

  handleNudge() {
    this.isNudged = true;
    setTimeout(() => this.isNudged = false, 2000);
  }
}
