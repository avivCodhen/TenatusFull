import { StockData } from './../_models/stockData';
import { HttpClient } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable, EventEmitter } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection: signalR.HubConnection;
  url = environment.apiUrl + 'stockdata';
  stockDataReceived = new Subject<any>();
  traderMessageReceived = new Subject<any>();
  constructor(private http: HttpClient) {}

  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/stockdata', {
        accessTokenFactory: () => localStorage.getItem('token'),
      })
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('connection started'))
      .catch((err) => console.log('error: ' + err));
  };

  addStockDataListener() {
    this.hubConnection.on('transferStockData', (data) => {
      this.stockDataReceived.next(data);
    });
    return this;
  }
  addTraderMessageListener() {
    this.hubConnection.on('consoleChannel', (data) => {
      console.log('traderMEssage: ' + data);
      this.traderMessageReceived.next(data);
    });
  }
  addOnCloseListener() {
    this.hubConnection.onclose((err) => {});
  }
}
