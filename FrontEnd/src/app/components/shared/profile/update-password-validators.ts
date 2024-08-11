import { AbstractControl, ValidatorFn } from '@angular/forms';

// Validator for newPassword strong
export function passwordValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const value: string = control.value;

    // Min lenght 8
    const minLength = 8;
    if (value.length < minLength) {
      return {
        passwordLength: {
          requiredLength: minLength,
          actualLength: value.length,
        },
      };
    }

    // Special char
    const specialCharacters = /[!@#$%^&*(),.?":{}|<>]/;
    if (!specialCharacters.test(value)) {
      return { passwordSpecialChars: true };
    }

    // Number
    const numbers = /\d/;
    if (!numbers.test(value)) {
      return { passwordNumbers: true };
    }

    // Upperkey
    const uppercaseLetters = /[A-Z]/;
    if (!uppercaseLetters.test(value)) {
      return { passwordUppercase: true };
    }

    return null;
  };
}
