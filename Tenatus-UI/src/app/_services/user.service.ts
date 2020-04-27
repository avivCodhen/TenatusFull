import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Dashboard } from '../_models/dashboard';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient) {}
  accountUrl = environment.apiUrl + 'accountSettings';
  dashboardUrl = environment.apiUrl + 'dashboard';

  getAccountSettings() {
    return this.http.get(this.accountUrl);
  }

  saveAccountSettings(model: any) {
    return this.http.post(this.accountUrl, model);
  }

  getDashBoardSetting() {
    return this.http.get<Dashboard>(this.dashboardUrl);
  }
}
