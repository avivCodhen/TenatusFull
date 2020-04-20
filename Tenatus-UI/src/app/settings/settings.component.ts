import { UserService } from './../_services/user.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss'],
})
export class SettingsComponent implements OnInit {
  model: any = {};
  constructor(private userService: UserService) {}

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
      (res) => console.log(res),
      (err) => console.log(err)
    );
  }
}
