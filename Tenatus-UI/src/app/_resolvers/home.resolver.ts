import { AlertService } from './../_services/alert.service';
import { UserService } from './../_services/user.service';
import { Dashboard } from './../_models/dashboard';
import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class HomeResolver implements Resolve<Dashboard> {
  constructor(
    private UserService: UserService,
    private alertService: AlertService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Dashboard> {
    return this.UserService.getDashBoardSetting().pipe(
      catchError((err) => {
        this.alertService.error(err);
        return of(null);
      })
    );
  }
}
