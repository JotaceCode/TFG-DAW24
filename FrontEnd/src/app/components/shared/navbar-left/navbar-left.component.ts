import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { MaterialModule } from '../../../material.module';
import { CommonModule } from '@angular/common';
import { SecurityService } from '../../../services/security.service';

@Component({
  selector: 'app-navbar-left',
  standalone: true,
  imports: [[MaterialModule, RouterModule, CommonModule, RouterOutlet]],
  templateUrl: './navbar-left.component.html',
  styleUrl: './navbar-left.component.css',
})
export class NavbarLeftComponent {
  constructor(private securityService: SecurityService) {}

  // Check if user is admin for show navbar links
  isAdmin() {
    return this.securityService.isAdmin();
  }
}
