import { Subject, Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PaginationRoom } from '../models/paginationRoom.model';
import { PaginationList } from '../models/Pagination.model';
import { Room } from '../models/room.model';

@Injectable({
  providedIn: 'root',
})
export class RoomService {
  baseUrl = environment.gatewayUrl;

  // Returned pagination model from request
  roomPagination: PaginationRoom | undefined;

  // Observable for send the room request data to components
  roomPaginationSubject = new Subject<PaginationRoom>();

  constructor(private http: HttpClient) {}

  // Search rooms with filters
  searchRooms(paginationRequest: PaginationList): void {
    this.http
      .post<PaginationRoom>(this.baseUrl + 'Room/pagination', paginationRequest)
      .subscribe((response) => {
        this.roomPagination = response;
        this.roomPaginationSubject.next(this.roomPagination);
      });
  }

  // Get all rooms
  getAllRooms(): Observable<Room[]> {
    return this.http.get<Room[]>(this.baseUrl + 'Room');
  }

  // Get an observable with the rooms
  getRooms(): Observable<PaginationRoom> {
    return this.roomPaginationSubject.asObservable();
  }

  // Delete room by id
  deleteRoom(id: string) {
    return this.http.delete(this.baseUrl + 'Room/' + id, {
      responseType: 'text',
    });
  }

  // Update the selected room
  updateRoom(id: string, updatedRoom: Room) {
    const body = { ...updatedRoom, id };
    return this.http.put(this.baseUrl + 'Room/' + id, body);
  }

  getRoomById(id: string) {
    return this.http.get(this.baseUrl + 'Room/' + id);
  }

  // insert a new room
  insertRoom(newRoom: Room) {
    return this.http.post<Room>(this.baseUrl + 'Room', newRoom);
  }
}
