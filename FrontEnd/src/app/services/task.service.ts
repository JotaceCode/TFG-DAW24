import { Subject, Observable, forkJoin } from 'rxjs';
import { environment } from '../../environment/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PaginationList } from '../models/Pagination.model';
import { PaginationTask } from '../models/paginationTask.model';
import { Task } from '../models/task.model';
import { pdfTask } from '../models/pdfTasks.model';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  baseUrl = environment.gatewayUrl;

  // Returned pagination model from request
  taskPagination: PaginationTask | undefined;
  taskHiddenPagination: PaginationTask | undefined;

  // Observable for send the task request data to components
  taskPaginationSubject = new Subject<PaginationTask>();
  taskHiddenPaginationSubject = new Subject<PaginationTask>();

  taskHistorySubject = new Subject<pdfTask>();
  taskHistory: pdfTask | undefined;

  constructor(private http: HttpClient) {}

  getAllHistory() {
    return this.taskHistorySubject.asObservable();
  }

  searchAllHistory() {
    this.http
      .get<pdfTask>(this.baseUrl + 'TaskHistory')
      .subscribe((response) => {
        this.taskHistory = response;
        this.taskHistorySubject.next(this.taskHistory);
      });
  }

  // Search tasks with filters
  searchTasks(paginationRequest: PaginationList): void {
    this.http
      .post<PaginationTask>(this.baseUrl + 'Task/pagination', paginationRequest)
      .subscribe((response) => {
        this.taskPagination = response;
        this.taskPaginationSubject.next(this.taskPagination);
      });
  }

  // Search all tasks with filters
  searchAllTasks(paginationRequest: PaginationList): void {
    this.http
      .post<PaginationTask>(this.baseUrl + 'Task/pagination', paginationRequest)
      .subscribe((response) => {
        this.taskHiddenPagination = response;
        this.taskHiddenPaginationSubject.next(this.taskHiddenPagination);
      });
  }
  // Get an observable with the tasks
  getTasks(): Observable<PaginationTask> {
    return this.taskPaginationSubject.asObservable();
  }
  // Get an observable with the hidden tasks
  getAllTasks(): Observable<PaginationTask> {
    return this.taskHiddenPaginationSubject.asObservable();
  }
  // Delete task by id
  deleteTask(id: string) {
    return this.http.delete(this.baseUrl + 'Task/' + id, {
      responseType: 'text',
    });
  }

  // Delete all the active tasks
  deleteAll() {
    return this.http.delete(this.baseUrl + 'Task/deleteAll', {
      responseType: 'text',
    });
  }

  // Update the selected task
  updateTask(id: string, updatedTask: Task) {
    const body = { ...updatedTask, id };
    return this.http.put(this.baseUrl + 'Task/' + id, body);
  }

  // insert a new task
  insertTask(newTask: Task) {
    return this.http.post<Task>(this.baseUrl + 'Task', newTask);
  }

  // Delete task from Task DB and inser it to TaskHistoryDB
  moveTaskToHistory(taskId: string, task: Task) {
    const deleteRequest = this.http.delete(this.baseUrl + 'Task/' + taskId, {
      responseType: 'text',
    });
    const postRequest = this.http.post<Task>(
      this.baseUrl + 'TaskHistory',
      task
    );

    return forkJoin([deleteRequest, postRequest]);
  }

  // Delete a task from the history
  deleteTaskFromHistory(id: string) {
    return this.http.delete(this.baseUrl + 'TaskHistory/' + id, {
      responseType: 'text',
    });
  }

  // Delete all the history
  deleteAllHistory() {
    return this.http.delete(this.baseUrl + 'TaskHistory/deleteHistory', {
      responseType: 'text',
    });
  }

  // Search tasks with filters
  searchTasksFromHistory(paginationRequest: PaginationList): void {
    this.http
      .post<PaginationTask>(
        this.baseUrl + 'TaskHistory/pagination',
        paginationRequest
      )
      .subscribe((response) => {
        this.taskPagination = response;
        this.taskPaginationSubject.next(this.taskPagination);
      });
  }

  // Get an observable with the tasks
  getTasksFromHistory(): Observable<PaginationTask> {
    return this.taskPaginationSubject.asObservable();
  }
}
