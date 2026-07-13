import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BurndownDay {
  date: string;
  completedTasksCount: number;
  remainingTasksCount: number;
}

export interface DashboardStats {
  openTasksCount: number;
  closedTasksCount: number;
  totalCompletedHours: number;
  burndownData: BurndownDay[];
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:8080/api/dashboard';

  getStats(projectId: string): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.API_URL}/stats`, {
      params: { projectId }
    });
  }
}
