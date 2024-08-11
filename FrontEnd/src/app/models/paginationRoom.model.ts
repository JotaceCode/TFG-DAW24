import { PaginationFilter } from './paginationFilter.model';
import { Room } from './room.model';

export interface PaginationRoom {
  pageSize: number;
  page: number;
  sort: string;
  sortDirection: string;
  pagesQuantity: number;
  data: Room[];
  filter: PaginationFilter[];
  totalRows: number;
}
