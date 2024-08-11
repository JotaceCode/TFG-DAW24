import { CommonModule } from '@angular/common';
import { SecurityService } from './services/security.service';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { Subscription } from 'rxjs';
import { NotificationService } from './services/notifications.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavbarLeftComponent } from './components/shared/navbar-left/navbar-left.component';
import { NavbarTopComponent } from './components/shared/navbar-top/navbar-top.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterOutlet, NavbarLeftComponent, NavbarTopComponent],
  templateUrl: './root.component.html',
  styleUrl: './root.component.css',
})
export class RootComponent implements OnInit {
  notifications: string[] = [];
  private notificationSubscription?: Subscription;

  constructor(
    private router: Router,
    private securityService: SecurityService,
    private notificationService: NotificationService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    const lastTask = this.notifications.pop();
    if (lastTask !== undefined) {
      this.showNotification(lastTask);
    }
    // Check if exist session token for load the account without login in any component
    if (typeof localStorage !== 'undefined') {
      if (localStorage.getItem('token')) {
        // In this method, is token expired, go to login anyway
        this.securityService.getLoggedUser();
        if (localStorage.getItem('rol') === "true") {
          this.notificationSubscription =
            this.notificationService.notifications$.subscribe(
              (notification) => {
                this.notifications.push(notification);
                this.showNotification(notification);
              }
            );
        }
      } else {
        // If there are no sesion, go to login
        this.router.navigate(['/']);
      }
    }
  }

  // Check if is login page for dont show navbar
  isLoginPage(): boolean {
    if (this.router.url === '/') {
      return true;
    } else {
      return false;
    }
  }

  showNotification(message: string): void {
    this.snackBar.open(message + ' terminó sus tareas', 'Cerrar', {
      duration: 5000,
      horizontalPosition: 'end',
      verticalPosition: 'bottom',
    });
  }
}
