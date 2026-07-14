import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:8080/api';

  getTask(taskId: string): Observable<any> {
    return this.http.get<any>(`${this.API_URL}/tasks/${taskId}`);
  }

  updateTask(taskId: string, taskData: {
    title: string;
    description: string;
    priority: number;
    assigneeId: string | null;
    reporterId: string | null;
    dueDate: string | null;
    estimatedHours: number | null;
  }): Observable<any> {
    return this.http.put<any>(`${this.API_URL}/tasks/${taskId}`, taskData);
  }

  deleteTask(taskId: string): Observable<any> {
    return this.http.delete<any>(`${this.API_URL}/tasks/${taskId}`);
  }

  getComments(taskId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.API_URL}/tasks/${taskId}/comments`);
  }

  addComment(taskId: string, content: string): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/tasks/${taskId}/comments`, { content });
  }

  getAttachments(taskId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.API_URL}/tasks/${taskId}/attachments`);
  }

  uploadAttachment(taskId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<any>(`${this.API_URL}/tasks/${taskId}/attachments`, formData);
  }

  deleteAttachment(attachmentId: string): Observable<any> {
    return this.http.delete<any>(`${this.API_URL}/attachments/${attachmentId}`);
  }

  createTask(taskData: any): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/tasks`, taskData);
  }

  getOrganizationMembers(orgId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.API_URL}/organizations/${orgId}/members`);
  }
}
