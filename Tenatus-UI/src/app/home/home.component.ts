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
  processing: boolean;
  constructor(
    private userService: UserService,
    private traderService: TraderService,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    this.getTraderSetting();
  }

  start() {
    this.processing = true;
    this.traderService.startTrader(this.tradingSetting).subscribe(
      (res) => {
        this.processing = false;
        console.log(res);
        this.alertService.success('Trader started successfully');
        this.getTraderSetting();
      },
      (err) => {
        this.processing = false;
        this.alertService.error('Error: ' + err);
        console.log(err);
      }
    );
  }

  stop() {
    this.traderService.stopTrader().subscribe(
      (res) => {
        console.log(res);
        this.alertService.success('Trader stopped successfully');
        this.getTraderSetting();
      },
      (err) => {
        this.alertService.error('Error: ' + err);
        console.log(err);
      }
    );
  }

  getTraderSetting() {
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
}
