import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SignalRService } from './signalr.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private readonly http = inject(HttpClient);
  private readonly signalRService = inject(SignalRService);
  private readonly API_URL = 'http://localhost:8080/api/notifications';

  readonly notifications = signal<any[]>([]);
  readonly unreadCount = computed(() => this.notifications().filter(n => !n.isRead).length);

  constructor() {
    this.loadNotifications();
    this.signalRService.connectToNotifications();
    
    // Listen for real-time notifications
    this.signalRService.notificationReceived$.subscribe((notif) => {
      this.notifications.update(list => [notif, ...list]);
    });
  }

  loadNotifications(): void {
    this.http.get<any[]>(this.API_URL).subscribe({
      next: (data) => {
        // Sort notifications by CreatedAt descending
        this.notifications.set(data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()));
      },
      error: (err) => console.error('Failed to load notifications:', err)
    });
  }

  markAsRead(id: string): Observable<any> {
    // Update local state immediately
    this.notifications.update(list =>
      list.map(n => n.id === id ? { ...n, isRead: true } : n)
    );

    return this.http.put<any>(`${this.API_URL}/${id}/read`, {});
  }

  markAllAsRead(): Observable<any> {
    // Update local state immediately
    this.notifications.update(list =>
      list.map(n => ({ ...n, isRead: true }))
    );

    return this.http.put<any>(`${this.API_URL}/read-all`, {});
  }
}
