import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TraderService {
  constructor(private http: HttpClient) {}

  url = environment.apiUrl + 'trader';

  startTrader() {
    return this.http.post(this.url + '/start', {});
  }
  stopTrader() {
    return this.http.delete(this.url + '/stop');
  }
}
