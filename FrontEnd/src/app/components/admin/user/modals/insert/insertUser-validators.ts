import { FormControl } from '@angular/forms';

export function dniValidator(control: FormControl) {
  const dniRegex = /^[0-9]{8}[TRWAGMYFPDXBNJZSQVHLCKET]$/i;

  const valid = dniRegex.test(control.value);

  return valid ? null : { invalidDni: true };
}
