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

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {}

  register() {
    this.authService.register(this.model).subscribe(
      (next) => {
        console.log('success');
      },
      (err) => {
        console.log(err);
      },
      () => {
        this.router.navigate(['home']);
      }
    );
  }
}
