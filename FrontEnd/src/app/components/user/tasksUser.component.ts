import { Component, OnInit } from '@angular/core';
import { delay, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../material.module';
import { Task } from '../../models/task.model';
import { TaskService } from '../../services/task.service';
import { SecurityService } from '../../services/security.service';
import { PaginationFilter } from '../../models/paginationFilter.model';
import { PaginationList } from '../../models/Pagination.model';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CompleteTasksUserModalComponent } from './modals/complete/completeTasksUserModal.component';
import { PaginationTask } from '../../models/paginationTask.model';
import { IncidenceTasksUserModalComponent } from './modals/incidence/incidenceTasksUserModal.component';

@Component({
  selector: 'app-tasks-users-board',
  standalone: true,
  imports: [MaterialModule, CommonModule],
  templateUrl: './tasksUser.component.html',
  styleUrls: ['./tasksUser.component.css'],
})
export class TasksUsersComponent implements OnInit {
  private taskSubscription: Subscription | undefined;
  dataSource: Task[] = [];
  userLoggedId: string | undefined;
  isLoading = false;
  constructor(
    private taskService: TaskService,
    private securityService: SecurityService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar
  ) {}

  // Dinamic filter to check more than one property and value
  paginationFilter: PaginationFilter[] = [
    {
      property: 'User.Id',
      value: '',
    },
  ];

  paginationRequest: PaginationList = {
    pageSize: 100,
    page: 1,
    sort: 'Status',
    sortDirection: 'desc',
    filter: this.paginationFilter,
  };

  ngOnInit(): void {
    this.userLoggedId = this.securityService.getUserLoggedId();
    this.paginationFilter[0].value = this.userLoggedId;

    this.taskService.searchTasks(this.paginationRequest);
    this.taskSubscription = this.taskService
      .getTasks()
      .subscribe((pagination: PaginationTask) => {
        this.dataSource = pagination.data;
      });
  }

  // Method that change the task status
  changeTaskStatus(task: Task) {
    if (task.status == 'Pendiente') {
      task.status = 'Finalizada';
    } else if (task.status == 'Finalizada') {
      task.status = 'Pendiente';
    }
    this.taskService.updateTask(task.id, task).subscribe(() => {
      this.taskService.searchTasks(this.paginationRequest);
    });
  }

  // Method that change the task incidence (observations)
  incidence(task: Task) {
    const dialogRef = this.dialog.open(IncidenceTasksUserModalComponent, {
      data: { task },
    });

    dialogRef
      .afterClosed()
      .pipe(delay(300))
      .subscribe((result) => {
        if (result === 'confirm') {
          this.taskService.updateTask(task.id, task).subscribe(() => {
            this.taskService.searchTasks(this.paginationRequest);
          });

          this.snackbar.open('Incidencia actualizada', 'Cerrar', {
            duration: 3000,
          });
        }
      });
  }
  // Method to put the completed task on historic task collection
  completeTaskList(taskList: Task[]) {
    const dialogRef = this.dialog.open(CompleteTasksUserModalComponent);

    dialogRef
      .afterClosed()
      .pipe(delay(300))
      .subscribe((result) => {
        if (result === 'confirm') {
          taskList.forEach((task) => {
            // Move task to history
            this.taskService.moveTaskToHistory(task.id, task).subscribe(() => {
              this.taskService.searchTasks(this.paginationRequest);
            });
          });
          this.snackbar.open('Tareas completadas', 'Cerrar', {
            duration: 3000,
          });
        }
      });
  }
}
