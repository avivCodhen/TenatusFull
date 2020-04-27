import { Dashboard } from './../_models/dashboard';
import { UserOrder } from './../_models/userOrder';
import { AlertService } from './../_services/alert.service';
import { TraderService } from './../_services/trader.service';
import { UserService } from './../_services/user.service';
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
  dashboard: Dashboard;
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
    this.getDashboard();

    this.dataSource = new MatTableDataSource<UserOrder>(
      this.dashboard.userOrders
    );
    this.dataSource.paginator = this.paginator;
  }
  getDashboard() {
    this.userService.getDashBoardSetting().subscribe(
      (res) => {
        this.dashboard = res;
        res.userOrders = [
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
      },
      (err) => {}
    );
  }

  start() {
    this.processing = true;
    this.traderService.startTrader().subscribe(
      (res) => {
        this.processing = false;
        console.log(res);
        this.alertService.success('Trader started successfully');
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
      },
      (err) => {
        this.alertService.error('Error: ' + err);
        console.log(err);
      }
    );
  }
}
