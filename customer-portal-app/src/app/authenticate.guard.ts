import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthenticateService } from './authenticate.service';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})

export class AuthenticateGuard implements CanActivate {
  constructor(private authService: AuthenticateService, private router: Router) {}

  canActivate() {
    return this.authService.getCurrentUserGuid().pipe(
      map((userGuid) => {
        if (userGuid) {
          return true;
        } else {
          this.router.navigate(['/signin']);
          return false;
        }
      })
    );
  }
}