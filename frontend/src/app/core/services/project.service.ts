import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:8080/api';

  getProjects(orgId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.API_URL}/projects?organizationId=${orgId}`);
  }

  createProject(orgId: string, projectData: { name: string; description?: string; color?: string; icon?: string }): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/projects`, { organizationId: orgId, ...projectData });
  }
}
