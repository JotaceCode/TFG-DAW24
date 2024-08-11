import { Component, OnInit, Output, EventEmitter, Inject } from '@angular/core';
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
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogRef,
} from '@angular/material/dialog';
import { InsertOrUpdateUser } from '../../../../../models/userUpdateOrInsert.model';
import { dniValidator } from '../insert/insertUser-validators';
import { SecurityService } from '../../../../../services/security.service';
import { User } from '../../../../../models/user.model';
import { UpdateUserPasswordModal } from './updateUserPasswordModal/updateUserPasswordModal.component';

@Component({
  selector: 'app-update-user-modal',
  standalone: true,
  templateUrl: 'updateUserModal.component.html',
  styleUrl: './updateUserModal.component.css',
  imports: [CommonModule, FormsModule, MaterialModule, ReactiveFormsModule],
})
export class UpdateUserModalComponent implements OnInit {
  userForm: FormGroup | any;
  @Output() modalClosed = new EventEmitter<void>();

  imageName: string = '';
  imageData: string | null = null;
  photo: File | any;

  constructor(
    private securityService: SecurityService,
    private userService: UserService,
    private formBuilder: FormBuilder,
    private _snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<UpdateUserModalComponent>,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: any
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
    });
    this.userForm.get('name')?.setValue(this.data.user.name);
    this.userForm.get('surname')?.setValue(this.data.user.surname);
    this.userForm.get('email')?.setValue(this.data.user.email);
    this.userForm.get('dni')?.setValue(this.data.user.dni);
    this.userForm.get('roleAdmin')?.setValue(this.data.user.roleAdmin);
    this.userForm.get('phoneNumber')?.setValue(this.data.user.phoneNumber);
    this.userForm.get('birthDate')?.setValue(this.data.user.birthDate);
    this.userForm.get('city')?.setValue(this.data.user.city);
    this.userForm.get('cp')?.setValue(this.data.user.cp);
    if (this.data.user.urlImage) {
      this.imageData = this.userService.getPhoto(this.data.user.urlImage);
    }
  }

  // Try to update the user with the new data
  updateUser(Id: string) {
    let birthDate: Date | null = this.userForm.get('birthDate')?.value;

    // 2 cases : date from db and date null from db
    if (typeof birthDate === 'string') {
      birthDate = new Date(birthDate);
    }

    if (!birthDate) {
      birthDate = null;
    }

    var userRequest: InsertOrUpdateUser = {
      Name: this.userForm.get('name')?.value,
      Surname: this.userForm.get('surname')?.value,
      Email: this.userForm.get('email')?.value,
      DNI: this.userForm.get('dni')?.value,
      PhoneNumber: this.userForm.get('phoneNumber')?.value,
      CP: this.userForm.get('cp')?.value,
      City: this.userForm.get('city')?.value,
      RoleAdmin: this.userForm.get('roleAdmin')?.value,
      urlImage: this.data.user.urlImage,
      Disabled:this.data.user.disabled
    };

    // manage the 2 cases : date from db and date null from db
    if (birthDate !== null) {
      if (birthDate instanceof Date) {
        // Ajusted to local date
        const birthDateLocal = new Date(
          birthDate.getTime() - birthDate.getTimezoneOffset() * 60000
        );
        userRequest.BirthDate = birthDateLocal.toISOString();
      } else {
        userRequest.BirthDate = birthDate;
      }
    }

    // 2 cases: delete last imagen from db or upload a new image
    if (this.imageData === '' && this.data.user.urlImage !== '') {
      userRequest.urlImage = '';
      this.userService.deletePhoto(this.data.user.urlImage).subscribe();
    }

    if (this.photo) {
      // Update user for set the url image = id user + format image if image is set
      const fileType = this.photo.type.split('/')[1]; // Get photo format
      const newName = Id + '.' + fileType; // Set new name (user id + photo format)
      // Create a new file with the new name and the image data
      const newPhoto = new File([this.photo], newName, {
        type: this.photo.type,
      });
      this.userService.deletePhoto(userRequest.urlImage).subscribe();
      userRequest.urlImage = newName;
      this.userService.uploadPhoto(newPhoto).subscribe();
    }

    this.userService.updateUser(Id, userRequest).subscribe({
      next: () => {
        this.openSnackBar('Usuario actualizado correctamente.');
      },
      error: (error) =>
        this.openSnackBar('Error al actualizar usuario ¿Email duplicado?' + error),
    });

    this.modalClosed.emit();
  }

  // Snackbar with the update password action result from the API
  openSnackBar(message: string) {
    this._snackBar.open(message, 'Cerrar', { duration: 3000 });
  }

  // Used for dont show some options for the user logged (for example, he cant change him rol)
  getUserLoggedId(): string {
    return this.securityService.getUserLoggedId();
  }

  updateUserPasswordModal(user: User) {
    this.dialog.open(UpdateUserPasswordModal, {
      width: '400px',
      data: { user },
    });
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
