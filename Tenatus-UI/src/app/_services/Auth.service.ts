import { AlertService } from './alert.service';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  constructor(private http: HttpClient, private router: Router) {}

  login(model: any) {
    return this.http.post(this.baseUrl + 'tokenSignin', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          this.router.navigate(['/home']);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'signup', model).pipe(
      map((response: any) => {
        console.log(response);
        this.login(model);
      })
    );
  }

  isLoggedIn() {
    const token = localStorage.getItem('token');
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }
}
