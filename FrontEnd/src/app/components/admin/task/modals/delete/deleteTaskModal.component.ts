import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../material.module';

@Component({
  selector: 'app-delete-task-modal',
  standalone: true,
  imports: [MaterialModule, CommonModule],
  templateUrl: './deleteTaskModal.component.html',
  styleUrls: ['./deleteTaskModal.component.css'],
})
export class DeleteTaskModalComponent {
  constructor(
    public dialogRef: MatDialogRef<DeleteTaskModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onConfirmClick(): void {
    this.dialogRef.close('confirm');
  }
}
