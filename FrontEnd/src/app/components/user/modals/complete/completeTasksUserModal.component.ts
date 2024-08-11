import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from '../../../../material.module';

@Component({
  selector: 'app-complete-user-modal',
  standalone: true,
  imports: [MaterialModule, CommonModule],
  templateUrl: './completeTasksUserModal.component.html',
  styleUrls: ['./completeTasksUserModal.component.css'],
})
export class CompleteTasksUserModalComponent {
  constructor(
    public dialogRef: MatDialogRef<CompleteTasksUserModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onConfirmClick(): void {
    this.dialogRef.close('confirm');
  }
}
