import { LoginData } from '../components/security/login-data.model';
import { User } from '../models/user.model';
import { Observable, Subject } from 'rxjs';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { environment } from '../../environment/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class SecurityService {
  baseUrl = environment.gatewayUrl;

  // Token and user role for manage the routes and permissions in the app
  private token: string | any;
  private userRole: boolean | any;
  private userId: string | any;

  // Subject for manage the login message in login.html
  securityChange = new Subject<boolean>();

  constructor(private router: Router, private http: HttpClient) {}

  // Login in the app, set cookies with the token and user rol
  login(loginData: LoginData) {
    this.http.post<User>(this.baseUrl + 'User/login', loginData).subscribe({
      next: (data) => {
        // Set login Subject successful
        this.securityChange.next(true);
        // Set cookies
        localStorage.setItem('token', data.token);
        localStorage.setItem('userId', data.id);
        localStorage.setItem('rol', data.roleAdmin.toString());
        localStorage.setItem('loginTime', new Date().toString()); // for destroy token with + 1 duration days
        // Set variables
        this.token = localStorage.getItem('token');
        this.userId = localStorage.getItem('userId');
        this.userRole = localStorage.getItem('rol');
        // With reload for admins for suscribe to signalR from backend
        this.router.navigate(['/inicio']).then(() => {
          window.location.reload();
        });
      },
      error: (e) => {
        // set login Subject failed
        this.securityChange.next(false);
        console.log(e);
      },
    });
  }

  // Get logged user in any component (used in root.component for load the data in each refresh)
  getLoggedUser(): Observable<User> | null {
    // If token have more than 1 day of lifetime, destroy it
    if (this.isTokenExpired()) {
      this.logout();
      return null;
    }

    // Set the variables with the cookies data
    const tokenBrowser = localStorage.getItem('token');
    const userId = localStorage.getItem('userId');
    const rol = localStorage.getItem('rol');
    this.token = tokenBrowser;
    this.userId = userId;
    this.userRole = rol;

    // Method can be used too for get the actual user data (in the profile component, for example)
    return this.http.post<User>(this.baseUrl + 'User/loggedUser', null);
  }

  // Logged user can update him password in the profile component
  updatePassword(oldPassword: string, newPassword: string): Observable<string> {
    // Model for change password in the API
    const body = {
      oldPassword: oldPassword,
      newPassword: newPassword,
    };

    // Return a response with the request result in text format (NO json)
    return this.http.post(this.baseUrl + 'User/updatePassword', body, {
      responseType: 'text',
    });
  }

  // admin users can reset a user password in the update user modal component
  updateUserPassword(userId: string, newPassword: string): Observable<string> {
    // Model for change password in the API
    const body = {
      newPassword: newPassword,
    };

    // Return a response with the request result in text format (NO json)
    return this.http.post(
      this.baseUrl + 'User/resetPassword/' + userId,
      body,
      {
        responseType: 'text',
      }
    );
  }

  // Navbar icon for close session, delete tokens
  logout() {
    this.securityChange.next(false);
    this.token = null;
    this.userRole = false;
    localStorage.removeItem('token');
    localStorage.removeItem('loginTime');
    this.router.navigate(['/']);
  }

  // Used in this.GetLoggedUser() for check if token expired
  isTokenExpired(): boolean {
    const loginTime = localStorage.getItem('loginTime');
    if (!loginTime) return true; // No login time recorded
    const now = new Date();
    const loginDateTime = new Date(loginTime);
    const expirationTime = loginDateTime.getTime() + 24 * 60 * 60 * 1000; // Add one day
    return now.getTime() > expirationTime;
  }

  // Used for Routes, only can navigate if the user is logged
  onSesion() {
    return this.token != null;
  }

  // Used for Routes, only admins can access to the components with this directive
  isAdmin(): boolean {
    if (this.userRole === 'true') {
      return true;
    } else {
      return false;
    }
  }

  // Used for dont show some options for the user logged (for example, he cant delete him user)
  getUserLoggedId(): string {
    return this.userId;
  }
}
