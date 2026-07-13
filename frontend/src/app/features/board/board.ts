import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { BoardStore } from '../../state/board.store';
import { SignalRService } from '../../core/services/signalr.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { TaskDetailDialogComponent } from './task-detail-dialog/task-detail-dialog';
import { CreateTaskDialogComponent } from './create-task-dialog/create-task-dialog';
import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { TaskService } from '../../core/services/task.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [
    CommonModule,
    DragDropModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatDialogModule,
    MatMenuModule
  ],
  templateUrl: './board.html',
  styleUrl: './board.scss'
})
export class BoardComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly boardStore = inject(BoardStore);
  private readonly signalRService = inject(SignalRService);
  private readonly dialog = inject(MatDialog);
  private readonly taskService = inject(TaskService);

  projectId: string = '';
  private signalRSub?: Subscription;
  private queryParamSub?: Subscription;

  // Compute connected list IDs for CDK drag-and-drop
  get connectedTo(): string[] {
    return this.boardStore.columns().map(c => `col-${c.id}`);
  }

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') || '';
    if (this.projectId) {
      this.boardStore.loadBoard(this.projectId);
      
      // Connect to SignalR board hub
      this.signalRService.connectToBoard(this.projectId);
      
      // Listen to real-time status updates from other users
      this.signalRSub = this.signalRService.taskStatusUpdates$.subscribe((update) => {
        this.boardStore.receiveTaskStatusUpdate(update.taskId, update.newStatus, update.newBoardColumnId);
      });
    }

    // Monitor query params to trigger task dialog (deep links & notifications)
    this.queryParamSub = this.route.queryParams.subscribe(params => {
      const taskId = params['taskId'];
      if (taskId) {
        this.openTaskDetails(taskId, false);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.projectId) {
      this.signalRService.disconnectFromBoard(this.projectId);
    }
    this.signalRSub?.unsubscribe();
    this.queryParamSub?.unsubscribe();
  }

  onDrop(event: CdkDragDrop<any[]>): void {
    if (event.previousContainer === event.container) {
      // Reordering in same list (local only)
      const data = event.container.data;
      const prevIdx = event.previousIndex;
      const currIdx = event.currentIndex;
      const item = data[prevIdx];
      data.splice(prevIdx, 1);
      data.splice(currIdx, 0, item);
    } else {
      const task = event.previousContainer.data[event.previousIndex];
      const newColumnId = event.container.id.replace('col-', '');
      const targetColumn = this.boardStore.columns().find(c => c.id === newColumnId);
      
      if (!targetColumn) return;
      const newStatus = targetColumn.name; // Set status equal to column name

      // Use optimistic update in store
      this.boardStore.updateTaskStatusOptimistic(task.id, newStatus, newColumnId);
    }
  }

  openTaskDetails(taskId: string, updateUrl = true): void {
    if (updateUrl) {
      this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { taskId },
        queryParamsHandling: 'merge'
      });
    }

    // Prevent opening multiple dialogs for same task
    const isAlreadyOpen = this.dialog.openDialogs.some(
      d => d.componentInstance instanceof TaskDetailDialogComponent && d.componentInstance.taskId === taskId
    );
    if (isAlreadyOpen) return;

    const dialogRef = this.dialog.open(TaskDetailDialogComponent, {
      width: '950px',
      maxWidth: '95vw',
      data: { taskId }
    });

    dialogRef.afterClosed().subscribe(() => {
      this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { taskId: null },
        queryParamsHandling: 'merge'
      });
    });
  }

  openCreateTaskDialog(columnId: string): void {
    this.dialog.open(CreateTaskDialogComponent, {
      width: '550px',
      data: { boardColumnId: columnId }
    });
  }

  getPriorityColor(priority: number): string {
    switch (priority) {
      case 3: return '#dc2626'; // Critical
      case 2: return '#ef4444'; // High
      case 1: return '#f97316'; // Medium
      default: return '#3b82f6'; // Low
    }
  }

  getPriorityLabel(priority: number): string {
    switch (priority) {
      case 3: return 'Critical';
      case 2: return 'Hoog';
      case 1: return 'Medium';
      default: return 'Laag';
    }
  }

  getPriorityClass(priority: number): string {
    switch (priority) {
      case 3: return 'priority-critical';
      case 2: return 'priority-high';
      case 1: return 'priority-medium';
      default: return 'priority-low';
    }
  }

  deleteTaskDirect(taskId: string): void {
    if (confirm('Weet je zeker dat je deze taak wilt verwijderen?')) {
      this.taskService.deleteTask(taskId).subscribe({
        next: () => {
          this.boardStore.loadBoard(this.boardStore.projectId());
        },
        error: () => alert('Fout bij het verwijderen van de taak.')
      });
    }
  }
}

