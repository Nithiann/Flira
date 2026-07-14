import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:8080/api';

  getTeams(orgId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.API_URL}/teams?organizationId=${orgId}`);
  }

  getTeamMembers(teamId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.API_URL}/teams/${teamId}/members`);
  }

  inviteMember(teamId: string, inviteData: { email: string; role: string }): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/teams/${teamId}/members`, { email: inviteData.email });
  }

  createTeam(orgId: string, teamData: { name: string; description?: string }): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/teams`, { organizationId: orgId, ...teamData });
  }
}
