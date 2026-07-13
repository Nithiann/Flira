import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserOrganization {
  id: string;
  name: string;
  role: string;
}

export interface UserProject {
  id: string;
  name: string;
  color: string;
  icon: string;
  organizationName: string;
}

export interface UserProfile {
  userId: string;
  username: string;
  email: string;
  globalRole: string;
  organizations: UserOrganization[];
  projects: UserProject[];
}

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:8080/api/users';

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.API_URL}/profile`);
  }
}
