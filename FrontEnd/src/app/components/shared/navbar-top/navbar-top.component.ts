import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { MaterialModule } from '../../../material.module';
import { CommonModule } from '@angular/common';
import { SecurityService } from '../../../services/security.service';
import { clockComponent } from './clock.component';
import { LogoutModalComponent } from './modal/logoutModal.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { delay } from 'rxjs';

@Component({
  selector: 'app-navbar-top',
  standalone: true,
  imports: [
    [MaterialModule, RouterModule, CommonModule, clockComponent, RouterOutlet],
  ],
  templateUrl: './navbar-top.component.html',
  styleUrl: './navbar-top.component.css',
})
export class NavbarTopComponent {
  constructor(
    private securityService: SecurityService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar
  ) {}

  // Logout, delete tokens
  logOut() {
    const dialogRef = this.dialog.open(LogoutModalComponent);
    dialogRef
      .afterClosed()
      .pipe(delay(300))
      .subscribe((result) => {
        if (result === 'confirm') {
          this.securityService.logout();
          this.snackbar.open('Sesión cerrada', 'Cerrar', {
            duration: 3000,
          });
        }
      });
  }

  // Check if user is admin for show navbar links
  isAdmin() {
    return this.securityService.isAdmin();
  }
}
