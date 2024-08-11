import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../material.module';

@Component({
  selector: 'app-logout-modal',
  standalone: true,
  imports: [MaterialModule, CommonModule],
  templateUrl: './logoutModal.component.html',
  styleUrls: ['./logoutModal.component.css'],
})
export class LogoutModalComponent {
  constructor(
    public dialogRef: MatDialogRef<LogoutModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onCancelClick(): void {
    this.dialogRef.close();
  }

  onConfirmClick(): void {
    this.dialogRef.close('confirm');
  }
}
