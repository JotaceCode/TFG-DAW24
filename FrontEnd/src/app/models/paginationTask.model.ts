import { PaginationFilter } from "./paginationFilter.model";
import { Task } from "./task.model";

export interface PaginationTask {
  pageSize: number;
  page: number;
  sort: string;
  sortDirection: string;
  pagesQuantity: number;
  data: Task[];
  filter: PaginationFilter;
  totalRows: number;
}
