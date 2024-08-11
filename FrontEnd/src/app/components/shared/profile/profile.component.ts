import { Component, OnInit } from '@angular/core';
import { User } from '../../../models/user.model';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../material.module';
import { SecurityService } from '../../../services/security.service';
import { MatDialog } from '@angular/material/dialog';
import { UpdatePasswordModal } from './update-password-modal';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  // Inicialize user for show some in the screen at the start (and for not get errors)
  userData: User = {
    id: '',
    name: '',
    surname: '',
    email: '',
    dni: '',
    phoneNumber: '',
    birthDate: new Date(Date.now()),
    cp: '',
    city: '',
    roleAdmin: true,
    token: '',
    urlImage: '',
  };

  constructor(
    private securityService: SecurityService,
    private userService: UserService,
    public dialog: MatDialog
  ) {}

  // Get userLogged data for show it
  ngOnInit(): void {
    this.securityService.getLoggedUser()?.subscribe(
      (response) => {
        this.userData = response;
      },
      (error) => {
        console.error('Error al obtener los datos del usuario:', error);
      }
    );
  }

  // UpdatePasswordDialog
  openDialog() {
    this.dialog.open(UpdatePasswordModal);
  }

  getPhoto() {
    return this.userService.getPhoto(this.userData.urlImage);
  }
}
