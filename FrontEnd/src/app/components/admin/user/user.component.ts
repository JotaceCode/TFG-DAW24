import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import {
  MatPaginator,
  MatPaginatorIntl,
  PageEvent,
} from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { Subscription, delay } from 'rxjs';
import { MaterialModule } from '../../../material.module';
import { MatTableDataSource } from '@angular/material/table';
import { Pagination } from '../../../models/Pagination.model';
import { PaginationFilter } from '../../../models/paginationFilter.model';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { User } from '../../../models/user.model';
import { UserService } from '../../../services/user.service';
import { PaginationUser } from '../../../models/paginationUser.model';
import { DeleteUserModalComponent } from './modals/delete/deleteUserModal.component';
import { SecurityService } from '../../../services/security.service';
import { InsertUserModalComponent } from './modals/insert/insertUserModal.component';
import { UpdateUserModalComponent } from './modals/update/updateUserModal.component';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { InsertOrUpdateUser } from '../../../models/userUpdateOrInsert.model';

@Component({
  selector: 'app-user-table',
  standalone: true,
  imports: [MaterialModule, CommonModule],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css',
})
export class UserTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort) ordering?: MatSort | any;
  @ViewChild(MatPaginator) pagination?: MatPaginator | any;

  private userSubscription: Subscription | undefined;

  searchRadioButtonValue = 'Name';

  dataSource = new MatTableDataSource<User>();
  totalUsers = 0;
  comboPages = [5, 10, 25, 50];
  displayedColumns = [
    'Name',
    'Surname',
    'Email',
    'PhoneNumber',
    'City',
    'actions',
  ];

  timeout: any = null;

  // Filter for search by column
  paginationFilter: PaginationFilter = {
    property: 'Name',
    value: '',
  };
  // Request for get users paginated with the filter (default null)
  paginationRequest: Pagination = {
    pageSize: 5,
    page: 1,
    sort: 'Name',
    sortDirection: 'asc',
    filter: this.paginationFilter,
  };

  constructor(
    private securityService: SecurityService,
    private userService: UserService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar,
    private paginatorIntl: MatPaginatorIntl
  ) {
    // Pagination in spanish
    this.paginatorIntl.nextPageLabel = 'Siguiente página';
    this.paginatorIntl.previousPageLabel = 'Página anterior';
    this.paginatorIntl.itemsPerPageLabel = 'Elementos por página:';
    this.paginatorIntl.getRangeLabel = (
      page: number,
      pageSize: number,
      length: number
    ) => {
      if (length === 0 || pageSize === 0) {
        return `0 de ${length}`;
      }
      length = Math.max(length, 0);
      const startIndex = page * pageSize;
      const endIndex =
        startIndex < length
          ? Math.min(startIndex + pageSize, length)
          : startIndex + pageSize;
      return `${startIndex + 1} - ${endIndex} de ${length}`;
    };
  }

  ngOnInit(): void {
    this.userService.searchUsers(this.paginationRequest);
    this.userSubscription = this.userService
      .getUsers()
      .subscribe((pagination: PaginationUser) => {
        this.dataSource = new MatTableDataSource<User>(pagination.data);
        this.totalUsers = pagination.totalRows;
      });
  }

  // Event from radio buttons for change column filter search
  onRadioButtonChange(event: any) {
    this.searchRadioButtonValue = event.value;

    var updatedPaginationFilter: PaginationFilter = {
      property: this.searchRadioButtonValue,
      value: this.paginationFilter.value,
    };
    this.paginationRequest.filter = updatedPaginationFilter;

    this.userService.searchUsers(this.paginationRequest);
  }

  // Event from search input for search by column and value
  userFilter(event: any): void {
    clearTimeout(this.timeout);
    const $this = this;

    this.timeout = setTimeout(() => {
      if (event.keyCode != 13) {
        $this.paginationFilter = {
          property: this.searchRadioButtonValue,
          value: event.target.value,
        };
        this.paginationRequest.filter = $this.paginationFilter;
        $this.userService.searchUsers(this.paginationRequest);
      }
    }, 500);
  }

  // For pagination and ordering (first charge)
  ngAfterViewInit(): void {
    this.dataSource.sort = this.ordering;
    this.dataSource.paginator = this.pagination;
  }

  // For pagination and ordering (bot options)
  eventPager(event: PageEvent): void {
    this.paginationRequest.pageSize = event.pageSize;
    this.paginationRequest.page = event.pageIndex + 1;
    this.userService.searchUsers(this.paginationRequest);
  }

  // For pagination and ordering (column options)
  orderColumns(event: Sort): void {
    this.paginationRequest.sort = event.active;
    this.paginationRequest.sortDirection = event.direction;
    this.userService.searchUsers(this.paginationRequest);
  }

  getOrderingTitle(): string {
    // Text for ordenation column
    const sortDirection = this.paginationRequest.sortDirection;
    if (sortDirection != '') {
      return sortDirection === 'asc' ? 'Orden Ascendente' : 'Orden Descendente';
    }
    return '';
  }

  // Open a modal for insert a new user
  insertUser(): void {
    const dialogRef = this.dialog.open(InsertUserModalComponent);
    dialogRef
      .afterClosed()
      .pipe(delay(300))
      .subscribe(() => {
        this.userService.searchUsers(this.paginationRequest);
      });
  }

  // Open a modal for watch or update user data
  updateUser(user: string): void {
    const dialogRef = this.dialog.open(UpdateUserModalComponent, {
      data: { user },
    });
    dialogRef
      .afterClosed()
      .pipe(delay(200))
      .subscribe(() => {
        this.userService.searchUsers(this.paginationRequest);
      });
  }

  // Open the modal with a confirmation to delete user
  deleteUser(userId: string, user: User): void {
    const dialogRef = this.dialog.open(DeleteUserModalComponent, {
      data: { userId, user },
    });

    // If modal was closed with a 'confirm' status delete the user
    dialogRef
      .afterClosed()
      .pipe(delay(200))
      .subscribe((result) => {
        if (result === 'confirm') {
          this.userService.deleteUser(userId).subscribe((response) => {
            if (user.urlImage) {
              this.userService.deletePhoto(user.urlImage).subscribe();
            }
            this.snackbar.open(response, 'Cerrar', { duration: 3000 });
            this.userService.searchUsers(this.paginationRequest);
          });
        }
      });
  }

  getUserLoggedId(): string {
    return this.securityService.getUserLoggedId();
  }

  // Get the user and open the profile modal view
  checkUser(userId: string): void {
    this.userService.getUserById(userId).subscribe((user: User) => {
      const dialogRef = this.dialog.open(UserProfileComponent, {
        data: { user },
      });

      dialogRef.afterClosed().subscribe(() => {
        this.userService.searchUsers(this.paginationRequest);
      });
    });
  }

  disableUser(userId: string, user: InsertOrUpdateUser) {
    user.Disabled = true;
    this.userService.updateUser(userId, user).subscribe(() => {
      this.userService.searchUsers(this.paginationRequest);
      this.snackbar.open('Usuario deshabilitado', 'Cerrar', { duration: 3000 });
    });
  }

  enableUser(userId: string, user: InsertOrUpdateUser) {
    user.Disabled = false;
    this.userService.updateUser(userId, user).subscribe(() => {
      this.userService.searchUsers(this.paginationRequest);
      this.snackbar.open('Usuario habilitado', 'Cerrar', { duration: 3000 });
    });
  }
}
