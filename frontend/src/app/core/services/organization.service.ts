import { Injectable, inject, signal, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:8080/api/organizations';
  private readonly ACTIVE_ORG_KEY = 'flira-active-org-id';

  readonly organizations = signal<any[]>([]);
  readonly activeOrganization = signal<any | null>(null);
  readonly currentUserRole = signal<string | null>(null);

  constructor() {
    // Automatically load organizations when service is created
    this.loadOrganizations().subscribe();
  }

  loadOrganizations(): Observable<any[]> {
    return this.http.get<any[]>(this.API_URL).pipe(
      tap(orgs => {
        this.organizations.set(orgs);
        if (orgs.length > 0) {
          const savedOrgId = localStorage.getItem(this.ACTIVE_ORG_KEY);
          const active = orgs.find(o => o.id === savedOrgId) || orgs[0];
          this.setActiveOrganization(active);
        } else {
          this.activeOrganization.set(null);
          this.currentUserRole.set(null);
        }
      })
    );
  }

  setActiveOrganization(org: any): void {
    if (org) {
      this.activeOrganization.set(org);
      this.currentUserRole.set(org.role || 'Member');
      localStorage.setItem(this.ACTIVE_ORG_KEY, org.id);
    } else {
      this.activeOrganization.set(null);
      this.currentUserRole.set(null);
      localStorage.removeItem(this.ACTIVE_ORG_KEY);
    }
  }

  createOrganization(orgData: { name: string; description?: string }): Observable<any> {
    return this.http.post<any>(this.API_URL, orgData).pipe(
      tap(() => this.loadOrganizations().subscribe())
    );
  }

  updateOrganization(id: string, orgData: { name: string; description?: string }): Observable<any> {
    return this.http.put<any>(`${this.API_URL}/${id}`, orgData).pipe(
      tap(() => {
        // Reload organizations list to refresh name/description
        this.loadOrganizations().subscribe();
      })
    );
  }

  deleteOrganization(id: string): Observable<any> {
    return this.http.delete<any>(`${this.API_URL}/${id}`).pipe(
      tap(() => {
        localStorage.removeItem(this.ACTIVE_ORG_KEY);
        this.loadOrganizations().subscribe();
      })
    );
  }

  getOrganizationMembers(id: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.API_URL}/${id}/members`);
  }

  addOrganizationMember(id: string, email: string, role: string): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/${id}/members`, { email, role });
  }

  removeOrganizationMember(id: string, userId: string): Observable<any> {
    return this.http.delete<any>(`${this.API_URL}/${id}/members/${userId}`);
  }

  updateOrganizationMemberRole(id: string, userId: string, role: string): Observable<any> {
    return this.http.put<any>(`${this.API_URL}/${id}/members/${userId}/role`, { role });
  }
}
