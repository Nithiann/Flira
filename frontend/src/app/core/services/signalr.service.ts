import { Injectable, inject } from '@angular/core';
import { AuthService } from './auth.service';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private readonly authService = inject(AuthService);
  private readonly HUB_BASE_URL = 'http://localhost:8080/hubs';

  private boardConnection: HubConnection | null = null;
  private notificationConnection: HubConnection | null = null;

  readonly taskStatusUpdates$ = new Subject<{ taskId: string; newStatus: string; newBoardColumnId: string }>();
  readonly notificationReceived$ = new Subject<any>();

  connectToBoard(boardId: string): void {
    if (this.boardConnection) {
      this.disconnectFromBoard(boardId);
    }

    const token = this.authService.getToken();
    if (!token) return;

    this.boardConnection = new HubConnectionBuilder()
      .withUrl(`${this.HUB_BASE_URL}/board`, {
        accessTokenFactory: () => token
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build();

    this.boardConnection.on('ReceiveTaskStatusUpdate', (data) => {
      this.taskStatusUpdates$.next(data);
    });

    this.boardConnection.start()
      .then(() => {
        this.boardConnection?.invoke('JoinBoard', boardId)
          .catch(err => console.error('Error invoking JoinBoard:', err));
      })
      .catch(err => console.error('Error starting SignalR Board connection:', err));
  }

  disconnectFromBoard(boardId: string): void {
    if (!this.boardConnection) return;

    this.boardConnection.invoke('LeaveBoard', boardId)
      .catch(err => console.error('Error invoking LeaveBoard:', err))
      .finally(() => {
        this.boardConnection?.stop()
          .then(() => {
            this.boardConnection = null;
          })
          .catch(err => console.error('Error stopping SignalR Board connection:', err));
      });
  }

  connectToNotifications(): void {
    if (this.notificationConnection) return;

    const token = this.authService.getToken();
    if (!token) return;

    this.notificationConnection = new HubConnectionBuilder()
      .withUrl(`${this.HUB_BASE_URL}/notifications`, {
        accessTokenFactory: () => token
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build();

    this.notificationConnection.on('ReceiveNotification', (data) => {
      this.notificationReceived$.next(data);
    });

    this.notificationConnection.start()
      .catch(err => console.error('Error starting SignalR Notifications connection:', err));
  }

  disconnectFromNotifications(): void {
    if (!this.notificationConnection) return;

    this.notificationConnection.stop()
      .then(() => {
        this.notificationConnection = null;
      })
      .catch(err => console.error('Error stopping SignalR Notifications connection:', err));
  }
}
