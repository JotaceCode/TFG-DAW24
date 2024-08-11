import { CanActivate, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { SecurityService } from '../../services/security.service';
import { Observable } from 'rxjs';

// Need user token to navigate in the routes
@Injectable({
  providedIn: 'root',
})
export class SecurityRouter implements CanActivate {
  constructor(
    private securityService: SecurityService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean> | boolean {
    if (this.securityService.onSesion()) {
      return true;
    } else {
      return false;
    }
  }
}

// Need user roles to navigate in some routes
@Injectable({
  providedIn: 'root',
})
export class SecurityRolesRouter implements CanActivate {
  constructor(
    private securityService: SecurityService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean> | boolean {
    if (this.securityService.isAdmin() == true) {
      return true;
    } else {
      this.router.navigate(['/inicio']);
      return false;
    }
  }
}

// Need user roles to navigate in some routes
@Injectable({
  providedIn: 'root',
})
export class SecurityNoRolesRouter implements CanActivate {
  constructor(
    private securityService: SecurityService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean> | boolean {
    if (this.securityService.isAdmin() == false) {
      return true;
    } else {
      this.router.navigate(['/inicio']);
      return false;
    }
  }
}