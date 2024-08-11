import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from '../../../../material.module';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-incidence-user-modal',
  standalone: true,
  imports: [MaterialModule, CommonModule, FormsModule],
  templateUrl: './incidenceTasksUserModal.component.html',
  styleUrls: ['./incidenceTasksUserModal.component.css'],
})
export class IncidenceTasksUserModalComponent {
  originalObservations: string = this.data.task.observations;
  incidence: string = '';
  haveIncidence: boolean;

  constructor(
    public dialogRef: MatDialogRef<IncidenceTasksUserModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.haveIncidence = this.data.task.observations.includes('\nINCIDENCIA: ');
    if (this.haveIncidence) {
      const indice = this.data.task.observations.indexOf('\nINCIDENCIA: ');
      this.originalObservations = this.originalObservations.substring(
        0,
        indice
      );
      this.incidence = this.data.task.observations.substring(
        indice + '\nINCIDENCIA: '.length
      );
    }
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onConfirmClick(): void {
    this.data.task.observations =
      this.originalObservations + '\nINCIDENCIA: ' + this.incidence;
    if (this.incidence == '') {
      this.data.task.observations = this.originalObservations;
    }
    this.dialogRef.close('confirm');
  }
}
