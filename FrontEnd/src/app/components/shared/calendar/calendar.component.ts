import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { MaterialModule } from '../../../material.module';
import { SecurityService } from '../../../services/security.service';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [MaterialModule, CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css'],
})
export class CalendarComponent {
  userForm: FormGroup | any;

  cuadranteFileName: string = '';

  imageName: string = '';
  imageData: string | null = null;
  photo: File | any;

  constructor(
    private securityService: SecurityService,
    private userService: UserService,
    private formBuilder: FormBuilder
  ) {
    this.userForm = this.formBuilder.group({
      urlImage: [''],
    });
    this.getAndUpdatePhoto();
  }

  uploadImage() {
    // 2 cases: delete last imagen from db or upload a new image
    if (this.imageData === null && this.cuadranteFileName !== '') {
      this.userService.deletePhoto('cuadrante').subscribe(() => {
        this.getAndUpdatePhoto();
      });
    }

    if (this.photo) {
      // Update user for set the url image = id user + format image if image is set
      const fileType = this.photo.type.split('/')[1]; // Get photo format
      const newName = 'cuadrante' + '.' + fileType; // Set new name (user id + photo format)
      // Create a new file with the new name and the image data
      const newPhoto = new File([this.photo], newName, {
        type: this.photo.type,
      });
      this.userService.uploadPhoto(newPhoto).subscribe(() => {
        this.getAndUpdatePhoto();
      });
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    this.photo = file;
    this.imageName = file.name;
    this.getImageData(file);
  }

  getImageData(file: File) {
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.imageData = e.target.result;
    };
    reader.readAsDataURL(file);
  }

  clearImage() {
    this.imageName = '';
    this.imageData = null;
    this.photo = '';
    this.userForm.get('urlImage').setValue('');
  }

  getPhoto() {
    return this.userService.getPhoto(this.cuadranteFileName);
  }

  getAndUpdatePhoto() {
    this.userService.getCuadranteFileName().subscribe((fileName) => {
      this.cuadranteFileName = fileName;
    });
  }

  // Check if user is admin for show upload photo form
  isAdmin() {
    return this.securityService.isAdmin();
  }
}
