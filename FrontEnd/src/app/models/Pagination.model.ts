import { PaginationFilter } from './paginationFilter.model';

export interface Pagination {
  pageSize: number;
  page: number;
  sort: string;
  sortDirection: string;
  filter?: PaginationFilter;
  exactValues?: boolean;
  exclude?: boolean;
}

export interface PaginationList {
  pageSize: number;
  page: number;
  sort: string;
  sortDirection: string;
  filter?: PaginationFilter[];
  exactValues?: boolean;
  exclude?: boolean;
}
