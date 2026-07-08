import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BoardService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:8080/api';

  getProjectBoard(projectId: string): Observable<any> {
    return this.http.get<any>(`${this.API_URL}/projects/${projectId}`);
  }

  getProjectTasks(projectId: string): Observable<any> {
    // Uses the SearchController endpoint to get all tasks for this project
    return this.http.get<any>(`${this.API_URL}/search/tasks`, {
      params: {
        projectId,
        pageSize: 500 // Return all tasks of the project
      }
    });
  }

  updateTaskStatus(taskId: string, status: string, boardColumnId: string): Observable<any> {
    return this.http.put<any>(`${this.API_URL}/tasks/${taskId}/status`, {
      newStatus: status,
      newBoardColumnId: boardColumnId
    });
  }
}
