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
  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {}

  login() {
    this.authService.login(this.model).subscribe(
      (next) => {
        console.log('success');
        this.router.navigate(['/home']);
      },
      (err) => {
        console.log('failed');
      }
    );
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }
}
