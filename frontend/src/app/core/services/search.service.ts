import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SearchTaskItem {
  id: string;
  boardColumnId: string;
  title: string;
  description: string;
  priority: number;
  status: string;
  assigneeId: string | null;
  reporterId: string | null;
  labels: string | null;
  dueDate: string | null;
  estimatedHours: number | null;
  createdAt: string;
}

export interface SearchResponse {
  items: SearchTaskItem[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface SearchFilters {
  searchTerm?: string;
  projectId?: string;
  assigneeId?: string;
  status?: string;
  priority?: number;
  labels?: string;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:8080/api/search';

  searchTasks(filters: SearchFilters): Observable<SearchResponse> {
    let params = new HttpParams();

    if (filters.searchTerm) params = params.set('searchTerm', filters.searchTerm);
    if (filters.projectId) params = params.set('projectId', filters.projectId);
    if (filters.assigneeId) params = params.set('assigneeId', filters.assigneeId);
    if (filters.status) params = params.set('status', filters.status);
    if (filters.priority !== undefined && filters.priority !== null) params = params.set('priority', filters.priority.toString());
    if (filters.labels) params = params.set('labels', filters.labels);
    if (filters.pageNumber) params = params.set('pageNumber', filters.pageNumber.toString());
    if (filters.pageSize) params = params.set('pageSize', filters.pageSize.toString());
    if (filters.sortBy) params = params.set('sortBy', filters.sortBy);
    if (filters.sortOrder) params = params.set('sortOrder', filters.sortOrder);

    return this.http.get<SearchResponse>(`${this.API_URL}/tasks`, { params });
  }
}
