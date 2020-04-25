import { UserOrder } from './../_models/userOrder';
import { AlertService } from './../_services/alert.service';
import { TraderService } from './../_services/trader.service';
import { UserService } from './../_services/user.service';
import { TraderSetting } from '../_models/traderSetting';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  dataSource: MatTableDataSource<UserOrder>;
  processing: boolean;
  tradingSetting: TraderSetting;
  userOrders: UserOrder[] = [];
  displayedColumns: string[] = [
    'Id',
    'Date',
    'Quantity',
    'Transaction Type',
    'Buy/Sell',
    'Price',
    'Stock',
  ];

  constructor(
    private userService: UserService,
    private traderService: TraderService,
    private alertService: AlertService
  ) {}

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngOnInit(): void {
    this.getTraderSetting();

    this.userOrders = [
      {
        externalId: '9823-102894-21=',
        created: '20/3/2020',
        price: 189,
        isBuy: false,
        orderAction: 'LMT',
        quantity: 1,
        stock: 'FB',
      },
    ];
    this.dataSource = new MatTableDataSource<UserOrder>(this.userOrders);
    this.dataSource.paginator = this.paginator;
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
