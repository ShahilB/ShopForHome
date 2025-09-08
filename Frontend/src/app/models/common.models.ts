export interface ApiResponse<T = any> {
  data?: T;
  message?: string;
  success: boolean;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface ErrorResponse {
  message: string;
  errors?: { [key: string]: string[] };
}

export interface LoadingState {
  isLoading: boolean;
  error?: string;
}

export interface SelectOption {
  value: any;
  label: string;
  disabled?: boolean;
}

export interface BreadcrumbItem {
  label: string;
  url?: string;
  active?: boolean;
}

export interface NotificationMessage {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  duration?: number;
  dismissible?: boolean;
}

export interface TableColumn {
  key: string;
  label: string;
  sortable?: boolean;
  width?: string;
  type?: 'text' | 'number' | 'date' | 'boolean' | 'currency' | 'image' | 'actions';
}

export interface SortOptions {
  field: string;
  direction: 'asc' | 'desc';
}

export interface FilterOption {
  key: string;
  label: string;
  type: 'text' | 'number' | 'select' | 'multiselect' | 'date' | 'boolean';
  options?: SelectOption[];
  value?: any;
}