import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MaterialModule } from '../../../material.module';
import { SecurityService } from '../../../services/security.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  standalone: true,
  imports: [MaterialModule, CommonModule, FormsModule, ReactiveFormsModule],
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loginstatus: boolean = true; // true default for not show error message at start

  // Validators
  constructor(
    private securityService: SecurityService,
    private formBuilder: FormBuilder,
    private router: Router
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      Password: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    // if start in login page with token, redirect to dashboard
    if (typeof localStorage !== 'undefined') {
      if (localStorage.getItem('token')) {
        this.router.navigate(['/inicio']);
      }
    }

    // suscribe to login status for show error message
    this.securityService.securityChange.subscribe((status: boolean) => {
      this.loginstatus = status;
    });
  }

  // Button for send user data from the login form
  login(form: FormGroup): void {
    this.securityService.login({
      Email: form.value.email,
      Password: form.value.Password,
    });
  }
}
