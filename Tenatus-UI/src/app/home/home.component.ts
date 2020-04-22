import { AlertService } from './../_services/alert.service';
import { TraderService } from './../_services/trader.service';
import { UserService } from './../_services/user.service';
import { TraderSetting } from './traderSetting';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  tradingSetting: TraderSetting;

  constructor(
    private userService: UserService,
    private traderService: TraderService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.userService.getTraderSetting().subscribe(
      (res) => {
        this.tradingSetting = res;
        console.log(res);
      },
      (err) => {
        console.log(err);
      }
    );
  }

  start() {
    this.traderService.startTrader(this.tradingSetting).subscribe(
      (res) => {
        console.log(res);
        this.alertService.success('Trader started successfully');
      },
      (err) => {
        console.log(err);
      }
    );
  }
}
