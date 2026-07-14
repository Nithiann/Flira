import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ColumnService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:8080/api/columns';

  createColumn(boardId: string, name: string): Observable<any> {
    return this.http.post<any>(this.API_URL, { boardId, name });
  }

  updateColumn(columnId: string, name: string): Observable<any> {
    return this.http.put<any>(`${this.API_URL}/${columnId}`, { name });
  }

  moveColumn(columnId: string, newPosition: number): Observable<any> {
    return this.http.put<any>(`${this.API_URL}/${columnId}/move`, { newPosition });
  }

  deleteColumn(columnId: string): Observable<any> {
    return this.http.delete<any>(`${this.API_URL}/${columnId}`);
  }
}
