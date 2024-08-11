import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../material.module';

@Component({
  selector: 'app-delete-user-modal',
  standalone: true,
  imports: [MaterialModule, CommonModule],
  templateUrl: './deleteUserModal.component.html',
  styleUrls: ['./deleteUserModal.component.css'],
})
export class DeleteUserModalComponent {
  constructor(
    public dialogRef: MatDialogRef<DeleteUserModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onConfirmClick(): void {
    this.dialogRef.close('confirm');
  }
}
