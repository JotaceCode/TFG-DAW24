import { Component, Inject, OnDestroy, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../material.module';
import { Subscription } from 'rxjs';
import { User } from '../../../../models/user.model';
import { UserService } from '../../../../services/user.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css',
})
export class UserProfileComponent implements OnInit, OnDestroy {
  userData: User = {
    id: '',
    name: '',
    surname: '',
    email: '',
    dni: '',
    phoneNumber: '',
    birthDate: new Date('2019-01-16'),
    cp: '',
    city: '',
    roleAdmin: false,
    token: '',
    urlImage: '',
  }; //Necesario para evitar error de lectura de un valor NULO
  userSubscription: Subscription | undefined;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private userService: UserService
  ) {}

  //Cuando creamos el componente hacemos una peticion para obtener el usuario
  ngOnInit(): void {
    this.userData = this.data.user;
  }

  //Cuando el compo se destruya, nos desubscribimos
  ngOnDestroy(): void {
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }
  getPhoto() {
    return this.userService.getPhoto(this.userData.urlImage);
  }
}
