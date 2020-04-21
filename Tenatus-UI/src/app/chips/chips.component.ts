import { Component, OnInit, Input } from '@angular/core';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { MatChipInputEvent } from '@angular/material/chips';

@Component({
  selector: 'app-chips',
  templateUrl: './chips.component.html',
  styleUrls: ['./chips.component.scss'],
})
export class ChipsComponent implements OnInit {
  constructor() {}

  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = true;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  @Input() stocks: string[] = [];

  add(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if (value || '') {
      this.stocks.push(value);
    }

    if (input) {
      input.value = '';
    }
  }

  remove(stock: string): void {
    const index = this.stocks.indexOf(stock);

    if (index >= 0) {
      this.stocks.splice(index, 1);
    }
  }

  ngOnInit(): void {}
}
