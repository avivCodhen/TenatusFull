import { AlertService } from './../_services/alert.service';
import { UserService } from './../_services/user.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss'],
})
export class SettingsComponent implements OnInit {
  model: any = {};
  tradingClients: string[] = ['Alpaca', 'Interactive'];

  constructor(
    private userService: UserService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.userService.getAccountSettings().subscribe(
      (res) => {
        this.model = res;
        console.log(res);
      },
      (err) => console.log(err)
    );
  }

  submit() {
    console.log(this.model);
    this.userService.saveAccountSettings(this.model).subscribe(
      (res) => {
        console.log(res);
        this.alertService.success('Settings Saved');
      },
      (err) => {
        console.log(err);
        this.alertService.error(err);
      }
    );
  }
}
