import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskService } from '../../core/services/task.service';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-task-resolve',
  standalone: true,
  imports: [CommonModule, MatProgressBarModule],
  template: `
    <div style="display: flex; flex-direction: column; align-items: center; justify-content: center; height: 100vh; font-family: sans-serif; color: #64748b;">
      <h3 style="margin-bottom: 1rem;">Taak openen...</h3>
      <mat-progress-bar mode="indeterminate" style="width: 250px;"></mat-progress-bar>
    </div>
  `
})
export class TaskResolveComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly taskService = inject(TaskService);

  ngOnInit(): void {
    const taskId = this.route.snapshot.paramMap.get('taskId');
    if (taskId) {
      this.taskService.getTask(taskId).subscribe({
        next: (task) => {
          if (task && task.projectId) {
            this.router.navigate(['/projects', task.projectId, 'board'], {
              queryParams: { taskId: task.id }
            });
          } else {
            console.error('Task or Project ID not found');
            this.router.navigate(['/dashboard']);
          }
        },
        error: (err) => {
          console.error('Error resolving task route:', err);
          this.router.navigate(['/dashboard']);
        }
      });
    } else {
      this.router.navigate(['/dashboard']);
    }
  }
}
