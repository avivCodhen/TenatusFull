import { TraderMessage } from './../_models/traderMessage';
import { StockData } from './../_models/stockData';
import { Dashboard } from './../_models/dashboard';
import { UserOrder } from './../_models/userOrder';
import { AlertService } from './../_services/alert.service';
import { TraderService } from './../_services/trader.service';
import { UserService } from './../_services/user.service';
import {
  Component,
  OnInit,
  ViewChild,
  ViewChildren,
  QueryList,
} from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { Strategy } from '../_models/strategy';
import { StrategyDialogComponent } from '../strategy-dialog/strategy-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { SignalRService } from '../_services/signalR.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  traderMessages: TraderMessage[] = [];
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
  stockDataSource: MatTableDataSource<StockData>;
  stockColumns: string[] = ['Open', 'Close', 'High', 'Low', 'Price', 'Name'];
  constructor(
    private userService: UserService,
    private traderService: TraderService,
    private alertService: AlertService,
    public dialog: MatDialog,
    private signalRService: SignalRService,
    private route: ActivatedRoute
  ) {
    this.signalRService.startConnection();
    this.signalRService.addStockDataListener().addTraderMessageListener();
  }

  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();

  ngOnInit(): void {
    this.dataSource = new MatTableDataSource<UserOrder>();
    this.strategyDataSource = new MatTableDataSource<Strategy>();
    this.stockDataSource = new MatTableDataSource<StockData>();
    this.getDashboard();
  }

  ngAfterViewInit() {
    this.strategyDataSource.paginator = this.paginator.toArray()[0];
    this.dataSource.paginator = this.paginator.toArray()[1];
  }
  getDashboard() {
    this.route.data.subscribe((data) => {
      this.dashboard = data['dashboard'];
      console.log(this.dashboard);
      this.strategyDataSource.data = this.dashboard.strategies;
      this.dataSource.data = this.dashboard.userOrders;
      this.stockDataSource.data = this.dashboard.stocks;

      this.signalRService.stockDataReceived.subscribe((data) => {
        console.log('signalR data:' + data.toString());
        this.dashboard.stocks.forEach((stock) => {
          if (data.stock.toUpperCase() == stock.stock.toUpperCase()) {
            stock.currentPrice = data.currentPrice;
          }
        });
      });

      this.signalRService.traderMessageReceived.subscribe(
        (data: TraderMessage) => {
          console.log('traderMessage: ' + data);
          this.traderMessages.unshift(data);
        }
      );
    });
  }

  start() {
    this.processing = true;
    this.traderService.startTrader().subscribe(
      (res) => {
        this.processing = false;
        console.log(res);
        this.dashboard.isTraderOn = true;
        this.alertService.success('Trader started successfully');
        this.dashboard.isTraderOn = true;
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
        this.dashboard.isTraderOn = false;
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
