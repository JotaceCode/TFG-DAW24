import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../material.module';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-addTaskAuto-task-modal',
  standalone: true,
  imports: [MaterialModule, CommonModule],
  templateUrl: './addTaskAutoTaskModal.component.html',
  styleUrls: ['./addTaskAutoTaskModal.component.css'],
})
export class AddTaskAutoTaskModalComponent {
  constructor(
    public dialogRef: MatDialogRef<AddTaskAutoTaskModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onConfirmClick(): void {
    this.dialogRef.close('confirm');
  }
}
