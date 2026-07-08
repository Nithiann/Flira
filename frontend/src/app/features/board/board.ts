import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BoardService } from '../../core/services/board.service';
import { CdkDragDrop, moveItemInArray, transferArrayItem, DragDropModule } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

interface ColumnWithTasks {
  id: string;
  name: string;
  position: number;
  tasks: any[];
}

@Component({
  selector: 'app-board',
  standalone: true,
  imports: [
    CommonModule,
    DragDropModule,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule
  ],
  templateUrl: './board.html',
  styleUrl: './board.scss'
})
export class BoardComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly boardService = inject(BoardService);

  projectId: string = '';
  projectName = signal<string>('');
  columns = signal<ColumnWithTasks[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  // Compute connected list IDs for CDK drag-and-drop
  readonly connectedTo = computed(() => this.columns().map(c => `col-${c.id}`));

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId') || '';
    if (this.projectId) {
      this.loadBoardData();
    }
  }

  loadBoardData(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    // Fetch board metadata
    this.boardService.getProjectBoard(this.projectId).subscribe({
      next: (projectData) => {
        this.projectName.set(projectData.name);
        
        // Find default board columns
        const board = projectData.boards?.[0];
        if (!board || !board.columns) {
          this.columns.set([]);
          this.isLoading.set(false);
          return;
        }

        const cols: ColumnWithTasks[] = board.columns.map((c: any) => ({
          id: c.id,
          name: c.name,
          position: c.position,
          tasks: []
        }));

        // Fetch tasks
        this.boardService.getProjectTasks(this.projectId).subscribe({
          next: (taskData) => {
            const taskItems = taskData.items || [];
            
            // Map tasks to columns based on boardColumnId
            cols.forEach(col => {
              col.tasks = taskItems.filter((t: any) => t.boardColumnId === col.id);
            });

            this.columns.set(cols.sort((a, b) => a.position - b.position));
            this.isLoading.set(false);
          },
          error: () => {
            this.isLoading.set(false);
            this.errorMessage.set('Fout bij het laden van taken.');
          }
        });
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Fout bij het laden van bord.');
      }
    });
  }

  onDrop(event: CdkDragDrop<any[]>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      // Optimistic update rollback backups
      const previousData = [...event.previousContainer.data];
      const currentData = [...event.container.data];

      const task = event.previousContainer.data[event.previousIndex];
      // container ID format: "col-{columnId}"
      const newColumnId = event.container.id.replace('col-', '');
      const targetColumn = this.columns().find(c => c.id === newColumnId);
      
      if (!targetColumn) return;
      const newStatus = targetColumn.name; // Set status equal to column name

      // Visually transfer item immediately
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );

      // Call API
      this.boardService.updateTaskStatus(task.id, newStatus, newColumnId).subscribe({
        next: () => {
          task.status = newStatus;
          task.boardColumnId = newColumnId;
        },
        error: (err) => {
          // API failed: Rollback immediate UI changes
          event.previousContainer.data.splice(0, event.previousContainer.data.length, ...previousData);
          event.container.data.splice(0, event.container.data.length, ...currentData);
          
          this.errorMessage.set(err.error?.Message || 'Fout bij het verplaatsen van taak.');
          setTimeout(() => this.errorMessage.set(null), 5000);
        }
      });
    }
  }

  getPriorityColor(priority: number): string {
    switch (priority) {
      case 2: return '#ef4444'; // High
      case 1: return '#f97316'; // Medium
      default: return '#3b82f6'; // Low
    }
  }
}
