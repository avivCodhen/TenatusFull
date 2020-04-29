import { Dashboard } from './../_models/dashboard';
import { UserOrder } from './../_models/userOrder';
import { AlertService } from './../_services/alert.service';
import { TraderService } from './../_services/trader.service';
import { UserService } from './../_services/user.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Strategy } from '../_models/strategy';
import { StrategyDialogComponent } from '../strategy-dialog/strategy-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  processing: boolean;
  dashboard: Dashboard;
  dataSource: MatTableDataSource<UserOrder>;
  displayedColumns: string[] = [
    'Date',
    'Quantity',
    'Transaction Type',
    'Buy/Sell',
    'Price',
    'Stock',
  ];
  strategyDataSource: MatTableDataSource<Strategy>;
  strategyColumns: string[] = [
    'Actions',
    'Active',
    'Date',
    'OrderType',
    'Budget',
    'Stock',
  ];
  constructor(
    private userService: UserService,
    private traderService: TraderService,
    private alertService: AlertService,
    public dialog: MatDialog
  ) {}

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatPaginator, { static: true }) strategyPaginator: MatPaginator;

  ngOnInit(): void {
    this.getDashboard();
  }
  getDashboard() {
    this.userService.getDashBoardSetting().subscribe(
      (res) => {
        this.dashboard = res;
        console.log(this.dashboard);
        this.dataSource = new MatTableDataSource<UserOrder>(
          this.dashboard.userOrders
        );
        this.dataSource.paginator = this.paginator;
        this.strategyDataSource = new MatTableDataSource<Strategy>(
          this.dashboard.strategies
        );
        this.strategyDataSource.paginator = this.paginator;
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
        this.dashboard.isOn = true;
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

  openDialog(): void {
    const dialogRef = this.dialog.open(StrategyDialogComponent, {
      data: {},
    });

    dialogRef.afterClosed().subscribe((result) => {
      console.log(result);
      if (!!result) {
        this.userService.saveStrategy(result).subscribe(
          (res) => {
            this.alertService.success('Strategy Saved');
            this.getStrategies();
          },
          (err) => {
            this.alertService.error('failed to Saved. Error: ' + err);
          }
        );
      }
    });
  }

  onCheck(data: any) {
    console.log(data);
    this.userService.editStrategy(data).subscribe(
      (res) => {
        this.alertService.success('Strategy Edited');
      },
      (err) => {
        this.alertService.error('failed to edit');
      }
    );
  }

  deleteStrategy(element: Strategy) {
    this.userService.deleteStrategy(element.id).subscribe(
      (res) => {
        this.alertService.success('Deleted successfully');
        this.getStrategies();
      },
      (err) => {
        this.alertService.error('Failed to delete');
      }
    );
  }
  getStrategies() {
    this.userService.getAllStrategies().subscribe(
      (res) => {
        this.dashboard.strategies = res;
      },
      (err) => {
        this.alertService.error('Error updating strategies. Err: ' + err);
      }
    );
  }
}
