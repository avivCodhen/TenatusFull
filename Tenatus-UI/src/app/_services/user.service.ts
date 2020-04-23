import { TraderSetting } from './../home/traderSetting';
import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient) {}
  accountUrl = environment.apiUrl + 'accountSettings';
  traderSettingUrl = environment.apiUrl + 'traderSetting';

  getAccountSettings() {
    return this.http.get(this.accountUrl);
  }

  saveAccountSettings(model: any) {
    return this.http.post(this.accountUrl, model);
  }

  getTraderSetting() {
    return this.http.get<TraderSetting>(this.traderSettingUrl);
  }
}
