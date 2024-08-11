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
  selector: 'app-insert-task-modal',
  standalone: true,
  templateUrl: 'insertTaskModal.component.html',
  styleUrl: './insertTaskModal.component.css',
  imports: [
    CommonModule,
    FormsModule,
    MaterialModule,
    ReactiveFormsModule,
    TaskUserTableComponent,
    TaskRoomTableComponent,
  ],
})
export class InsertTaskModalComponent implements OnInit {
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
    this.taskForm = this.formBuilder.group({
      user: ['', Validators.required],
      room: ['', Validators.required],
      priority: ['1'],
      status: ['Pendiente'],
      observations: [''],
    });
  }

  // Try to insert a new task a get a toast with the response
  insertTask() {
    var user: TaskUser = { Id: this.userId };
    var room: TaskRoom = {
      Id: this.roomId,
      roomNumber: '',
    };
    const taskRequest: Task = {
      User: user,
      Room: room,
      Priority: this.taskForm.get('priority')?.value,
      Status: 'Pendiente',
      Observations: this.taskForm.get('observations')?.value,
    };
    this.taskService.insertTask(taskRequest).subscribe({
      next: () => this.openSnackBar('Tarea añadida'),
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
}
