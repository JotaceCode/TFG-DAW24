import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../../material.module';


@Component({
  selector: 'app-task-observations-modal',
  standalone: true,
  imports: [MaterialModule, CommonModule],
  templateUrl: './TaskObservationsModal.component.html',
  styleUrls: ['./TaskObservationsModal.component.css'],
})
export class TaskObservationsModalComponent {
  constructor(
    public dialogRef: MatDialogRef<TaskObservationsModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCloseClick(): void {
    this.dialogRef.close();
  }
}
