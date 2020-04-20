import { AuthGuard } from './_guards/auth.guard';
import { RegisterComponent } from './register/register.component';
import { HomeComponent } from './home/home.component';
import { Routes } from '@angular/router';
import { SettingsComponent } from './settings/settings.component';

export const appRoutes: Routes = [
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'register', component: RegisterComponent },
  { path: 'settings', component: SettingsComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: 'home', pathMatch: 'full' },
];
