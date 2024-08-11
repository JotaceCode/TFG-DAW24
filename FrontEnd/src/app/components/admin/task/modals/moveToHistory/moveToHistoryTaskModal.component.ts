import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../material.module';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-moveToHistory-task-modal',
  standalone: true,
  imports: [MaterialModule, CommonModule],
  templateUrl: './moveToHistoryTaskModal.component.html',
  styleUrls: ['./moveToHistoryTaskModal.component.css'],
})
export class MoveToHistoryTaskModalComponent {
  constructor(
    public dialogRef: MatDialogRef<MoveToHistoryTaskModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onConfirmClick(): void {
    this.dialogRef.close('confirm');
  }
}
