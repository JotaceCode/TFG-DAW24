import { Component } from '@angular/core';
import { MaterialModule } from '../../../material.module';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { passwordValidator } from './update-password-validators';
import { SecurityService } from '../../../services/security.service';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { passwordMatchValidator } from './update-password-confirmation-validator';

@Component({
  selector: 'update-password-dialog',
  templateUrl: 'update-password-modal.html',
  standalone: true,
  imports: [MaterialModule, CommonModule, FormsModule, ReactiveFormsModule],
})
export class UpdatePasswordModal {
  passwordForm: FormGroup;
  snackbarDuration = 5;

  // Validators
  constructor(
    private formBuilder: FormBuilder,
    private securityService: SecurityService,
    private _snackBar: MatSnackBar
  ) {
    this.passwordForm = this.formBuilder.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, passwordValidator()]],
      repeatNewPassword: [
        '',
        [Validators.required, passwordMatchValidator('newPassword')],
      ],
    });
    this.subscribeToNewPasswordChanges(); // Subcription from newpassword observer for repeatNewPassword
  }

  // Update Password button aaction
  updatePassword() {
    const oldPassword = this.passwordForm.get('oldPassword')?.value;
    const newPassword = this.passwordForm.get('newPassword')?.value;
    this.securityService.updatePassword(oldPassword, newPassword).subscribe(
      () => {
        this.openSnackBar('Contraseña actualizada con éxito');
        console.log('Contraseña actualizada con éxito');
      },
      (error) => {
        this.openSnackBar('Error al actualizar la contraseña');
        console.error('Error al actualizar la contraseña:', error);
      }
    );
  }

  // Snackbar with the update password action result from the API
  openSnackBar(message: string) {
    this._snackBar.open(message, 'Cerrar', { duration: 3000 });
  }

  // Observer for synchronize newPassword and repeatNewPassword in both inputs
  subscribeToNewPasswordChanges() {
    const newPasswordControl = this.passwordForm.get('newPassword');
    if (newPasswordControl) {
      newPasswordControl.valueChanges.subscribe(() => {
        const repeatNewPasswordControl =
          this.passwordForm.get('repeatNewPassword');
        if (repeatNewPasswordControl) {
          repeatNewPasswordControl.updateValueAndValidity();
        }
      });
    }
  }
}
