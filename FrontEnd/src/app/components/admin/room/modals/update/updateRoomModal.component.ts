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
import { RoomService } from '../../../../../services/room.service';
import { Room } from '../../../../../models/room.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-update-room-modal',
  standalone: true,
  templateUrl: 'updateRoomModal.component.html',
  styleUrl: './updateRoomModal.component.css',
  imports: [CommonModule, FormsModule, MaterialModule, ReactiveFormsModule],
})
export class UpdateRoomModalComponent implements OnInit {
  roomForm: FormGroup | any;
  statusOptions = [
    { label: 'Vacía', value: 'Vacía' },
    { label: 'Ocupada', value: 'Ocupada' },
  ];
  typeOptions = [
    { label: 'Individual', value: 'Individual' },
    { label: 'Doble', value: 'Doble' },
    { label: 'Triple', value: 'Triple' },
    { label: 'Minusválidos', value: 'Minusválidos' },
    { label: 'Otro', value: 'Otro' },
  ];
  @Output() modalClosed = new EventEmitter<void>();

  constructor(
    private roomService: RoomService,
    private formBuilder: FormBuilder,
    private _snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<UpdateRoomModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  // Validators
  ngOnInit() {
    this.roomForm = this.formBuilder.group({
      roomNumber: ['', Validators.required],
      floor: [''],
      type: [''],
      status: [''],
    });
    this.roomForm.get('roomNumber')?.setValue(this.data.room.roomNumber);
    this.roomForm.get('floor')?.setValue(this.data.room.floor);
    this.roomForm.get('type')?.setValue(this.data.room.type);
    this.roomForm.get('status')?.setValue(this.data.room.status);
  }

  // Try to update the room with the new data
  updateRoom(roomId: string) {
    const roomRequest: Room = {
      RoomNumber: this.roomForm.get('roomNumber')?.value,
      Floor: this.roomForm.get('floor')?.value,
      Type: this.roomForm.get('type')?.value,
      Status: this.roomForm.get('status')?.value,
    };
    this.roomService.updateRoom(roomId, roomRequest).subscribe({
      next: () => this.openSnackBar('Habitación actualizada correctamente.'),
      error: () =>
        this.openSnackBar('Error al actualizar la habitación ¿Nº habitación duplicado?'),
    });
    this.modalClosed.emit();
  }

  // Snackbar with the update password action result from the API
  openSnackBar(message: string) {
    this._snackBar.open(message, 'Cerrar', { duration: 3000 });
  }
}
