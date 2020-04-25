import { AlertService } from './../_services/alert.service';
import { AuthService } from './../_services/Auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
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
  register() {
    this.authService.register(this.model).subscribe(
      (next) => {
        console.log('success');
        this.authService.login(this.model);
      },
      (err) => {
        this.alertService.error(err);
        console.log(err);
      },
      () => {
        this.router.navigate(['home']);
      }
    );
  }
}
