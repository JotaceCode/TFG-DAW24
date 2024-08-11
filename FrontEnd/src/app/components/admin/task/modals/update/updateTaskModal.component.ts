import { Component, OnInit, Output, EventEmitter, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormsModule,
  FormBuilder,
  Validators,
  FormGroup,
} from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms'; // Importa ReactiveFormsModule
import { MaterialModule } from '../../../../../material.module';
import { TaskService } from '../../../../../services/task.service';
import { Task, TaskUser, TaskRoom } from '../../../../../models/task.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TaskUserTableComponent } from '../tables/user/userTable.component';
import { TaskRoomTableComponent } from '../tables/room/roomTable.component';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-update-task-modal',
  standalone: true,
  templateUrl: 'updateTaskModal.component.html',
  styleUrl: './updateTaskModal.component.css',
  imports: [
    CommonModule,
    FormsModule,
    MaterialModule,
    ReactiveFormsModule,
    TaskUserTableComponent,
    TaskRoomTableComponent,
  ],
})
export class UpdateTaskModalComponent implements OnInit {
  taskForm: FormGroup | any;
  @Output() modalClosed = new EventEmitter<void>();

  userId: string = '';
  roomId: string = '';

  constructor(
    private taskService: TaskService,
    private formBuilder: FormBuilder,
    private _snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  // Validators
  ngOnInit() {
    const index = this.data.roomNumbers.indexOf(this.data.task.room.roomNumber);
    if (index !== -1) {
      this.data.roomNumbers.splice(index, 1);
    }
    this.userId = this.data.task.user.id;
    this.roomId = this.data.task.room.id;
    this.taskForm = this.formBuilder.group({
      user: [this.userId, Validators.required],
      room: [this.roomId, Validators.required],
      priority: [this.data.task.priority],
      status: [this.data.task.status],
      observations: [this.data.task.observations],
    });
  }

  // Try to update a new task a get a toast with the response
  updateTask() {
    const statusTask: string = this.data.task.status;
    var user: TaskUser = { Id: this.userId };
    var room: TaskRoom = {
      Id: this.roomId,
      roomNumber: '',
    };
    const taskRequest: Task = {
      User: user,
      Room: room,
      Priority: this.taskForm.get('priority')?.value,
      Status: statusTask,
      Observations: this.taskForm.get('observations')?.value,
    };
    this.taskService.updateTask(this.data.task.id, taskRequest).subscribe({
      next: () => this.openSnackBar('Tarea actualizada correctamente.'),
      error: () =>
        this.openSnackBar(
          'Error al añadir la tarea ¿La habitación ya está asignada?'
        ),
    });
    this.modalClosed.emit();
  }

  // Snackbar with the update password action result from the API
  openSnackBar(message: string) {
    this._snackBar.open(message, 'Cerrar', { duration: 3000 });
  }

  // Output from table with the selected user id
  onSelectedIdUserChange(selectedId: string) {
    this.userId = selectedId;
    this.taskForm.patchValue({
      user: this.userId,
    });
  }

  // Output from table with the selected user id
  onSelectedIdRoomChange(selectedId: string) {
    this.roomId = selectedId;
    this.taskForm.patchValue({
      room: this.roomId,
    });
  }

  // For button that update the status
  updateStatus(status: string) {
    this.data.task.status = status;

    const statusTask: string = this.data.task.status;
    var user: TaskUser = { Id: this.data.task.user.id };
    var room: TaskRoom = {
      Id: this.data.task.room.id,
      roomNumber: '',
    };
    const taskRequest: Task = {
      User: user,
      Room: room,
      Priority: this.data.task.priority,
      Status: statusTask,
      Observations: this.data.task.observations,
    };
    this.taskService.updateTask(this.data.task.id, taskRequest).subscribe({
      next: () => this.openSnackBar('Estado actualizado correctamente.'),
      error: () =>
        this.openSnackBar(
          'Error al actualizar el estado de la tarea'
        ),
    });
    this.modalClosed.emit();
  }
}
