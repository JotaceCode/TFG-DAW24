import {
  AfterViewInit,
  Component,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import {
  MatPaginator,
  MatPaginatorIntl,
  PageEvent,
} from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
import { Subscription, delay } from 'rxjs';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../../../material.module';
import { PaginationFilter } from '../../../../../../models/paginationFilter.model';
import { PaginationList } from '../../../../../../models/Pagination.model';
import { TaskService } from '../../../../../../services/task.service';
import { PaginationTask } from '../../../../../../models/paginationTask.model';
import { DeleteTaskModalComponent } from '../../delete/deleteTaskModal.component';
import { Task } from '../../../../../../models/task.model';
import { TaskObservationsModalComponent } from './TaskObservationsModal.component';
import { pdfTask } from '../../../../../../models/pdfTasks.model';

@Component({
  selector: 'app-task-history-table',
  standalone: true,
  imports: [MaterialModule, CommonModule],
  templateUrl: './taskHistory.component.html',
  styleUrl: './taskHistory.component.css',
})
export class TaskHistoryTableComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort) ordering?: MatSort | any;
  @ViewChild(MatPaginator) pagination?: MatPaginator | any;

  private taskSubscription: Subscription | undefined;

  private taskHistorySubscription: Subscription | undefined;

  pdfTasks: pdfTask | any;

  showActiveTasks: boolean = true;
  @Output() showActiveTasksChange = new EventEmitter<void>();

  searchRadioButtonValue = 'Status';
  searchDateDefaultValue: Date;

  dataSource = new MatTableDataSource<Task>();
  totalTasks = 0;
  comboPages = [5, 10, 25, 50];
  displayedColumns = [
    'Status',
    'Priority',
    'User.Email',
    'Room.RoomNumber',
    'CreatedDate',
    'actions',
  ];

  timeout: any = null;

  // Filter for search by column
  paginationFilter: PaginationFilter[] = [
    {
      property: 'Status',
      value: '',
    },
  ];
  // Request for get tasks paginated with the filter (default null)
  paginationRequest: PaginationList = {
    pageSize: 5,
    page: 1,
    sort: 'Status',
    sortDirection: 'asc',
    filter: this.paginationFilter,
  };

  constructor(
    private taskService: TaskService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar,
    private paginatorIntl: MatPaginatorIntl
  ) {
    this.searchDateDefaultValue = new Date(Date.now());
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
    this.taskService.searchAllHistory();
    this.taskSubscription = this.taskService
      .getAllHistory()
      .subscribe((pdfTasks: pdfTask) => {
        this.pdfTasks = pdfTasks;
      });

    this.taskService.searchTasksFromHistory(this.paginationRequest);
    this.taskSubscription = this.taskService
      .getTasksFromHistory()
      .subscribe((pagination: PaginationTask) => {
        this.dataSource = new MatTableDataSource<Task>(pagination.data);
        this.totalTasks = pagination.totalRows;
      });
  }

  // Event from radio buttons for change column filter search
  onRadioButtonChange(event: any) {
    this.searchRadioButtonValue = event.value;

    var updatedPaginationFilter: PaginationFilter[] = [
      {
        property: this.searchRadioButtonValue,
        value: this.paginationFilter[0].value,
      },
    ];
    if (event.value === 'Room.RoomNumber') {
      updatedPaginationFilter = this.searchByRoom(
        this.paginationFilter[0].value
      );
    }
    if (event.value === 'User.Email') {
      updatedPaginationFilter = this.searchByUser(
        this.paginationFilter[0].value
      );
    }
    if (event.value === 'CreatedDate') {
      updatedPaginationFilter = this.searchByDate(
        this.searchDateDefaultValue.toISOString()
      );
    }

    this.paginationRequest.filter = updatedPaginationFilter;

    // For reset filter when change from date to other checkbox
    const date = new Date(this.paginationFilter[0].value);
    if (event.value !== 'CreatedDate' && !isNaN(date.getTime())) {
      this.paginationRequest.filter = [
        {
          property: event.value,
          value: '',
        },
      ];
    }

    this.taskService.searchTasksFromHistory(this.paginationRequest);
  }

  // Event from search input for search by column and value
  taskFilter(event: any): void {
    clearTimeout(this.timeout);
    const $this = this;

    this.timeout = setTimeout(() => {
      if (event.keyCode != 13) {
        $this.paginationFilter = [
          {
            property: this.searchRadioButtonValue,
            value: event.target.value,
          },
        ];
        if (this.searchRadioButtonValue === 'Room.RoomNumber') {
          $this.paginationFilter = this.searchByRoom(event.target.value);
        }
        if (this.searchRadioButtonValue === 'User.Email') {
          $this.paginationFilter = this.searchByUser(event.target.value);
        }
        if (this.searchRadioButtonValue === 'CreatedDate') {
          var selectedDate = new Date(event.target.value);
          selectedDate.setDate(selectedDate.getDate() + 1);

          $this.paginationFilter = this.searchByDate(selectedDate.toISOString());
        }
        this.paginationRequest.filter = $this.paginationFilter;
        $this.taskService.searchTasksFromHistory(this.paginationRequest);
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
    this.taskService.searchTasksFromHistory(this.paginationRequest);
  }

  // For pagination and ordering (column options)
  orderColumns(event: Sort): void {
    this.paginationRequest.sort = event.active;
    this.paginationRequest.sortDirection = event.direction;
    this.taskService.searchTasksFromHistory(this.paginationRequest);
  }

  getOrderingTitle(): string {
    // Text for ordenation column
    const sortDirection = this.paginationRequest.sortDirection;
    if (sortDirection != '') {
      return sortDirection === 'asc' ? 'Orden Ascendente' : 'Orden Descendente';
    }
    return '';
  }

  showObservations(task: Task) {
    if (task.observations) {
      this.dialog.open(TaskObservationsModalComponent, {
        data: { task },
      });
    }
  }

  // Open the modal with a confirmation to delete task
  deleteTask(taskId: string, task: Task): void {
    const dialogRef = this.dialog.open(DeleteTaskModalComponent, {
      data: { taskId, task, message: ' del histórico de tareas' },
    });

    // If modal was closed with a 'confirm' status delete the task
    dialogRef
      .afterClosed()
      .pipe(delay(500))
      .subscribe((result) => {
        if (result === 'confirm') {
          this.taskService
            .deleteTaskFromHistory(taskId)
            .subscribe((response) => {
              // And show a snackbar with the request result
              this.snackbar.open(response, 'Cerrar', { duration: 3000 });
              // Then, get updated list tasks
              this.taskService.searchTasksFromHistory(this.paginationRequest);
              this.taskService.searchAllHistory();
            });
        }
      });
  }

  // Change the table that is showing
  toggleShowActiveTasks() {
    this.showActiveTasks = !this.showActiveTasks;
    this.taskService.searchTasks(this.paginationRequest);
  }

  // Event with the value to the parent
  emitShowActiveTasksChange() {
    this.showActiveTasksChange.emit();
  }

  // Filters used for dinamyc filter
  private searchByRoom(value: string) {
    const updatedPaginationFilter: PaginationFilter[] = [
      {
        property: 'Room.RoomNumber',
        value: value,
      },
      {
        property: 'Room.Floor',
        value: value,
      },
      {
        property: 'Room.Type',
        value: value,
      },
      {
        property: 'Room.Status',
        value: value,
      },
    ];
    return updatedPaginationFilter;
  }

  private searchByUser(value: string) {
    const updatedPaginationFilter: PaginationFilter[] = [
      {
        property: 'User.Name',
        value: value,
      },
      {
        property: 'User.Surname',
        value: value,
      },
      {
        property: 'User.Email',
        value: value,
      },
      {
        property: 'User.City',
        value: value,
      },
      {
        property: 'User.PhoneNumber',
        value: value,
      },
    ];
    return updatedPaginationFilter;
  }

  private searchByDate(value: string) {
    const updatedPaginationFilter: PaginationFilter[] = [
      {
        property: 'CreatedDate',
        value: value,
      },
    ];
    return updatedPaginationFilter;
  }
}
