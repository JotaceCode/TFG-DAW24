import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MaterialModule } from '../../../../../material.module';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-delete-history-modal',
  standalone: true,
  imports: [MaterialModule, CommonModule, FormsModule],
  templateUrl: './deleteHistoryModal.component.html',
  styleUrls: ['./deleteHistoryModal.component.css'],
})
export class DeleteHistoryModalComponent {
  confirmText: string = '';

  constructor(
    public dialogRef: MatDialogRef<DeleteHistoryModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onConfirmClick(): void {
    this.dialogRef.close('confirm');
  }
}
