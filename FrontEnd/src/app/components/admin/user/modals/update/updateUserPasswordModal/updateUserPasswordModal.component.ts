import { Component, Inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SecurityService } from '../../../../../../services/security.service';
import { passwordValidator } from '../../../../../shared/profile/update-password-validators';
import { passwordMatchValidator } from '../../../../../shared/profile/update-password-confirmation-validator';
import { MaterialModule } from '../../../../../../material.module';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'update-user-password-dialog',
  templateUrl: 'updateUserPasswordModal.component.html',
  standalone: true,
  imports: [MaterialModule, CommonModule, FormsModule, ReactiveFormsModule],
})
export class UpdateUserPasswordModal {
  passwordForm: FormGroup;
  snackbarDuration = 5;

  // Validators
  constructor(
    private formBuilder: FormBuilder,
    private securityService: SecurityService,
    private _snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.passwordForm = this.formBuilder.group({
      newPassword: ['', [Validators.required, passwordValidator()]],
      repeatNewPassword: [
        '',
        [Validators.required, passwordMatchValidator('newPassword')],
      ],
    });
    this.subscribeToNewPasswordChanges(); // Subcription from newpassword observer for repeatNewPassword
  }

  // Update Password button aaction
  updatePassword(userId: string) {
    const Password = this.passwordForm.get('repeatNewPassword')?.value;
    this.securityService.updateUserPassword(userId, Password).subscribe(
      () => {
        this.openSnackBar('Contraseña actualizada con éxito.');
      },
      (error) => {
        this.openSnackBar('Error al actualizar la contraseña.');
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
