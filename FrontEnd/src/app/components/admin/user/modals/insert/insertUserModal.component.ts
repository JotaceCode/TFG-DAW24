import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormsModule,
  FormBuilder,
  Validators,
  FormGroup,
} from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms'; // Importa ReactiveFormsModule
import { MaterialModule } from '../../../../../material.module';
import { UserService } from '../../../../../services/user.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { dniValidator } from './insertUser-validators';
import { InsertOrUpdateUser } from '../../../../../models/userUpdateOrInsert.model';
import { passwordValidator } from '../../../../shared/profile/update-password-validators';
import { passwordMatchValidator } from '../../../../shared/profile/update-password-confirmation-validator';

@Component({
  selector: 'app-insert-user-modal',
  standalone: true,
  templateUrl: 'insertUserModal.component.html',
  styleUrl: './insertUserModal.component.css',
  imports: [CommonModule, FormsModule, MaterialModule, ReactiveFormsModule],
})
export class InsertUserModalComponent implements OnInit {
  userForm: FormGroup | any;
  @Output() modalClosed = new EventEmitter<void>();
  imageName: string = '';
  imageData: string | null = null;
  photo: File | any;

  constructor(
    private userService: UserService,
    private formBuilder: FormBuilder,
    private _snackBar: MatSnackBar
  ) {}

  // Validators
  ngOnInit() {
    this.userForm = this.formBuilder.group({
      name: ['', Validators.required],
      surname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      dni: ['', [Validators.required, dniValidator]],
      roleAdmin: [false, Validators.required],
      phoneNumber: [''],
      birthDate: [''],
      city: [''],
      cp: [''],
      urlImage: [''],
      password: ['', [Validators.required, passwordValidator()]],
      repeatPassword: [
        '',
        [Validators.required, passwordMatchValidator('password')],
      ],
    });
    this.subscribeToPasswordChanges(); // Subcription from password observer for repeatPassword
  }

  // Try to insert a new user a get a toast with the response
  insertUser() {
    let birthDate: Date | null = this.userForm.get('birthDate')?.value;

    if (!birthDate) {
      birthDate = null;
    }

    const userRequest: InsertOrUpdateUser = {
      Name: this.userForm.get('name')?.value,
      Surname: this.userForm.get('surname')?.value,
      Email: this.userForm.get('email')?.value,
      DNI: this.userForm.get('dni')?.value,
      RoleAdmin: this.userForm.get('roleAdmin')?.value,
      PhoneNumber: this.userForm.get('phoneNumber')?.value,
      City: this.userForm.get('city')?.value,
      CP: this.userForm.get('cp')?.value,
      Password: this.userForm.get('password')?.value,
      urlImage: this.imageName,
    };

    if (birthDate !== null) {
      // Ajust time depending of time zone
      const birthDateLocal = new Date(
        birthDate.getTime() - birthDate.getTimezoneOffset() * 60000
      );
      userRequest.BirthDate = birthDateLocal.toISOString();
    }

    this.userService.insertUser(userRequest).subscribe({
      next: (user) => {
        if (this.photo) {
          // Update user for set the url image = id user + format image if image is set
          var newUser: InsertOrUpdateUser = userRequest;
          const fileType = this.photo.type.split('/')[1]; // Get photo format
          const newName = user.id + '.' + fileType; // Set new name (user id + photo format)
          // Create a new file with the new name and the image data
          const newPhoto = new File([this.photo], newName, {
            type: this.photo.type,
          });
          // Send it to the server
          newUser.urlImage = newName;
          this.userService.updateUser(user.id, newUser).subscribe();
          this.userService.uploadPhoto(newPhoto).subscribe();
        }

        this.openSnackBar('Usuario añadido correctamente.');
      },
      error: () => {
        this.openSnackBar('Error al añadir el usuario ¿email duplicado?');
      },
    });

    this.modalClosed.emit();
  }

  // Snackbar with the update password action result from the API
  openSnackBar(message: string) {
    this._snackBar.open(message, 'Cerrar', { duration: 3000 });
  }

  // Observer for synchronize password and repeatPassword in both inputs
  subscribeToPasswordChanges() {
    const passwordControl = this.userForm.get('password');
    if (passwordControl) {
      passwordControl.valueChanges.subscribe(() => {
        const repeatPasswordControl = this.userForm.get('repeatPassword');
        if (repeatPasswordControl) {
          repeatPasswordControl.updateValueAndValidity();
        }
      });
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    this.photo = file;
    this.imageName = file.name;
    this.getImageData(file);
  }

  getImageData(file: File) {
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.imageData = e.target.result;
    };
    reader.readAsDataURL(file);
  }

  clearImage() {
    this.imageName = '';
    this.imageData = '';
    this.photo = '';
    this.userForm.get('urlImage').setValue('');
  }
}
