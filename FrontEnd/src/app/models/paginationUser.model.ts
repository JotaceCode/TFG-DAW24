import { PaginationFilter } from './paginationFilter.model';
import { User } from './user.model';

export interface PaginationUser {
  pageSize: number;
  page: number;
  sort: string;
  sortDirection: string;
  pagesQuantity: number;
  data: User[];
  filter: PaginationFilter;
  totalRows: number;
}
