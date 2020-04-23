import { TraderSetting } from './../home/traderSetting';
import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TraderService {
  constructor(private http: HttpClient) {}

  url = environment.apiUrl + 'trader';

  startTrader(model: TraderSetting) {
    return this.http.post(this.url + '/start', model);
  }
  stopTrader() {
    return this.http.delete(this.url + '/stop');
  }
}
