import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../material.module';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-delete-all-modal',
  standalone: true,
  imports: [MaterialModule, CommonModule, FormsModule],
  templateUrl: './deleteAllModal.component.html',
  styleUrls: ['./deleteAllModal.component.css'],
})
export class DeleteAllModalComponent {
  confirmText: string = '';

  constructor(
    public dialogRef: MatDialogRef<DeleteAllModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onConfirmClick(): void {
    this.dialogRef.close('confirm');
  }
}
