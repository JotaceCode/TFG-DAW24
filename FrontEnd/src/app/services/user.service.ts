import { Subject, Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Pagination } from '../models/Pagination.model';
import { PaginationUser } from '../models/paginationUser.model';
import { User } from '../models/user.model';
import { InsertOrUpdateUser } from '../models/userUpdateOrInsert.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl = environment.gatewayUrl;
  photoUrl = environment.securityUrl;

  // Returned pagination model from request
  userPagination: PaginationUser | undefined;

  // Observable for send the user request data to components
  userPaginationSubject = new Subject<PaginationUser>();

  constructor(private http: HttpClient) {}

  // Search users with filters
  searchUsers(paginationRequest: Pagination): void {
    this.http
      .post<PaginationUser>(this.baseUrl + 'User/pagination', paginationRequest)
      .subscribe((response) => {
        this.userPagination = response;
        this.userPaginationSubject.next(this.userPagination);
      });
  }

  // Get all users
  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'User');
  }

  // Get an observable with the users
  getUsers(): Observable<PaginationUser> {
    return this.userPaginationSubject.asObservable();
  }

  // Delete user by id
  deleteUser(id: string) {
    return this.http.delete(this.baseUrl + 'User/' + id, {
      responseType: 'text',
    });
  }

  // Update the selected user
  updateUser(Id: string, updatedUser: InsertOrUpdateUser) {
    const body = { ...updatedUser, Id };
    return this.http.put(this.baseUrl + 'User/' + Id, body);
  }

  // insert a new user
  insertUser(newUser: InsertOrUpdateUser) {
    return this.http.post<User>(this.baseUrl + 'User', newUser);
  }

  uploadPhoto(photo: any) {
    const formData = new FormData();
    formData.append('photo', photo);

    return this.http.post(this.baseUrl + 'User/uploadPhoto', formData, {
      responseType: 'text',
    });
  }

  deletePhoto(fileName: string) {
    return this.http.delete(
      this.baseUrl + `User/deletePhoto?fileName=${fileName}`,
      {
        responseType: 'text',
      }
    );
  }

  getPhoto(urlImage: string): string {
    return this.baseUrl + 'photos/' + urlImage;
  }

  getUserById(id: string) {
    return this.http.get<User>(this.baseUrl + 'User/' + id);
  }

  getCuadranteFileName() {
    return this.http.get(this.baseUrl + 'User/getCuadranteFileName', {
      responseType: 'text',
    });
  }
}
