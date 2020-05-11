import { Strategy } from './../_models/strategy';
import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-strategy-dialog',
  templateUrl: './strategy-dialog.component.html',
  styleUrls: ['./strategy-dialog.component.scss'],
})
export class StrategyDialogComponent implements OnInit {
  constructor(
    public dialogRef: MatDialogRef<StrategyDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Strategy
  ) {
    this.strategyType = this.data.type;
    this.data.userOrderType = 'lmt';
  }

  strategyType: string;

  ngOnInit(): void {}

  close() {
    this.dialogRef.close();
  }
  save() {
    this.data.type = this.strategyType;
    this.dialogRef.close(this.data);
  }
}
