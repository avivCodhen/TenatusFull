import { AlertService } from './../_services/alert.service';
import { Router } from '@angular/router';
import { AuthService } from './../_services/Auth.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(
    private authService: AuthService,
    private router: Router,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {}

  login() {
    this.authService.login(this.model).subscribe(
      (next) => {
        console.log('success');
      },
      (err) => {
        this.alertService.error(err);
        console.log('failed');
      }
    );
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }
}
