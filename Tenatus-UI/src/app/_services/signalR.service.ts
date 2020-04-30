import { HttpClient } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection: signalR.HubConnection;
  url = environment.apiUrl + 'stockdata';

  constructor(private http: HttpClient) {}

  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/stockdata')
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('connection started'))
      .catch((err) => console.log('error: ' + err));
  };

  addTransferChartDataListener = () => {
    this.hubConnection.on('transferStockData', (data) => {
      console.log('signalR message: ' + data);
    });
  };

  start = () => {
    return this.http.get(this.url).subscribe();
  };
}
