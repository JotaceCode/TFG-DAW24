import { AbstractControl, ValidatorFn } from '@angular/forms';

// Validator for newPassword and repeat newPassword
export function passwordMatchValidator(controlName: string): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const parentControl = control.parent;
    if (!parentControl) {
      return null;
    }

    const passwordControl = parentControl.get(controlName);
    if (!passwordControl) {
      return null;
    }

    const password = passwordControl.value;

    if (password !== control.value) {
      control.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    } else {
      control.setErrors(null);
      return null;
    }
  };
}
