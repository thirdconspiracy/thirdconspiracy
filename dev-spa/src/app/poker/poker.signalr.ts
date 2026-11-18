import {Injectable} from '@angular/core';
import {HubConnection, HubConnectionBuilder} from '@microsoft/signalr';
import {BehaviorSubject, Subject} from 'rxjs';
import {PokerTable} from '../models/poker-table.model';
import {Player} from '../models/player.model';
import {Estimate} from '../models/estimate.enum';

@Injectable({providedIn: 'root'})
export class PokerSignalr {
  private connection!: HubConnection;
  private tableSubject = new BehaviorSubject<PokerTable | undefined>(undefined);
  public table$ = this.tableSubject.asObservable();

  private currentPlayerSubject = new BehaviorSubject<Player | undefined>(undefined);
  public currentPlayer$ = this.currentPlayerSubject.asObservable();

  private nudgeSubject = new Subject<void>();
  public nudge$ = this.nudgeSubject.asObservable();

  private tableId!: string;
  private playerName!: string;

  constructor() {
  }

  async initConnection(tableId: string, playerName: string) {
    this.tableId = tableId;
    this.playerName = playerName;

    if (this.connection && this.connection.state === 'Connected') {
      return;
    }

    this.connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5182/poker')
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveTable', (table: PokerTable) => {
      this.tableSubject.next(table);
      if (this.connection.connectionId) {
        this.currentPlayerSubject.next(table.players[this.connection.connectionId]);
      }
      this.checkAndAutoShow(table);
    });

    this.connection.on('ReceiveDeal', () => {
      const currentTable = this.tableSubject.value;
      if (currentTable) {
        Object.values(currentTable.players).forEach(player => {
          player.estimate = Estimate.None;
        });
        this.tableSubject.next({...currentTable});
        if (this.connection.connectionId) {
          this.currentPlayerSubject.next(currentTable.players[this.connection.connectionId])
        }
      }
    });

    this.connection.on('ReceivePlayerChange', (player: Player) =>{
      const currentTable = this.tableSubject.value;
      if (currentTable) {
        currentTable.players[player.connectionId] = player;
        this.tableSubject.next({...currentTable});
        if (this.connection.connectionId) {
          this.currentPlayerSubject.next(currentTable.players[this.connection.connectionId]);
        }
        this.checkAndAutoShow(currentTable);
      }
    });

    this.connection.on('ReceiveShow', (show: boolean) => {
      const currentTable = this.tableSubject.value;
      if (currentTable) {
        currentTable.show = show;
        this.tableSubject.next({...currentTable});
        if (this.connection.connectionId) {
          this.currentPlayerSubject.next(currentTable.players[this.connection.connectionId]);
        }
      }
    });


    this.connection.on('ReceiveDisconnected', (connectionId: string) => {
      const currentTable = this.tableSubject.value;
      if (currentTable) {
        delete currentTable.players[connectionId];
        this.tableSubject.next({...currentTable});
        if (this.connection.connectionId === connectionId){
          this.currentPlayerSubject.next(undefined);
        }
      }
    });

    this.connection.on('ReceiveNudge', () => {
      this.nudgeSubject.next();
    });

    try{
      await this.connection.start();
      console.log('Connection started');
      await this.connection
        .invoke('JoinTable', this.tableId, this.playerName);
      console.log('Joined table: ', this.tableId);
    } catch (err) {
      console.error('Connection failed', err);
    }
  }

  private checkAndAutoShow(table: PokerTable) {
    const activePlayers = Object.values(table.players).filter(player => player.isPlaying);
    const hasActivePlayers = activePlayers.length > 0;
    const allActivePlayersEstimated = hasActivePlayers && activePlayers.every(player => player.estimate !== Estimate.None);

    if (allActivePlayersEstimated && !table.show) {
      this.notifyShow(true);
    }

    if (hasActivePlayers && !allActivePlayersEstimated && !table.show) {
      this.notifyShow(false);
    }
  }

  async notifyShow(show: boolean) {
    try {
      await this.connection.invoke('NotifyShow', this.tableId, show);
    } catch (error) {
      console.error('NotifyShow failed', error);
    }
  }

  async notifyDeal() {
    try{
      await this.connection.invoke('NotifyDeal', this.tableId);
    } catch (err) {
      console.error('NotifyDeal failed', err);
    }
  }

  async notifyIsPlaying(isPlaying: boolean) {
    try{
      await this.connection.invoke('NotifyIsPlaying', this.tableId, isPlaying);
    } catch (err){
      console.error('NotifyIsPlaying failed', err);
    }
  }

  async notifyEstimate(estimate: Estimate) {
    try{
      await this.connection.invoke('NotifyEstimate', this.tableId, estimate);
    } catch (err){
      console.error('NotifyEstimate failed', err);
    }
  }

  async makeObserver(connectionId: string) {
    try {
      await this.connection.invoke('NotifySub', this.tableId, connectionId);
    } catch (err) {
      console.error('NotifySub failed', err);
    }
  }

  async notifyNudge(connectionId: string) {
    try{
      await this.connection.invoke('NotifyNudge', this.tableId, connectionId);
    } catch (err) {
      console.log('NotifyNudge failed', err);
    }
  }

  async disconnect() {
    if (this.connection){
      try {
        await this.connection.stop();
      } catch (err) {
        console.error('Error stopping SignalR connection', err);
      }
      this.connection = undefined as any;
    }
  }
}
