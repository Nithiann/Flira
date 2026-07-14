import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly API_URL = 'http://localhost:8080/api/auth'; // fallback url, will be parameterized

  private readonly TOKEN_KEY = 'flira-token';
  private readonly REFRESH_TOKEN_KEY = 'flira-refresh-token';

  readonly currentUser = signal<any | null>(null);
  readonly isAuthenticated = signal<boolean>(false);

  constructor() {
    this.checkAuthentication();
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/login`, credentials).pipe(
      tap(res => {
        if (res && res.token) {
          localStorage.setItem(this.TOKEN_KEY, res.token);
          localStorage.setItem(this.REFRESH_TOKEN_KEY, res.refreshToken);
          this.checkAuthentication();
        }
      })
    );
  }

  register(userData: any): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/register`, userData);
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/forgot-password`, { email });
  }

  resetPassword(data: any): Observable<any> {
    return this.http.post<any>(`${this.API_URL}/reset-password`, data);
  }

  refreshToken(): Observable<any> {
    const token = this.getToken();
    const refreshToken = this.getRefreshToken();
    
    return this.http.post<any>(`${this.API_URL}/refresh`, { token, refreshToken }).pipe(
      tap(res => {
        if (res && res.token) {
          localStorage.setItem(this.TOKEN_KEY, res.token);
          localStorage.setItem(this.REFRESH_TOKEN_KEY, res.refreshToken);
          this.checkAuthentication();
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
  }

  private checkAuthentication(): void {
    const token = this.getToken();
    if (token) {
      try {
        const decoded = this.decodeToken(token);
        // Check expiration
        const isExpired = decoded.exp * 1000 < Date.now();
        if (!isExpired) {
          this.currentUser.set(decoded);
          this.isAuthenticated.set(true);
        } else {
          this.logout();
        }
      } catch (e) {
        this.logout();
      }
    } else {
      this.currentUser.set(null);
      this.isAuthenticated.set(false);
    }
  }

  private decodeToken(token: string): any {
    const parts = token.split('.');
    if (parts.length !== 3) {
      throw new Error('Invalid JWT token');
    }
    const decoded = atob(parts[1].replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(decoded);
  }
}
