import { Component, Inject, OnInit, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { TaskService } from '../../../core/services/task.service';
import { OrganizationService } from '../../../core/services/organization.service';
import { AuthService } from '../../../core/services/auth.service';
import { BoardStore } from '../../../state/board.store';

@Component({
  selector: 'app-create-task-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatOptionModule
  ],
  templateUrl: './create-task-dialog.html',
  styleUrl: './create-task-dialog.scss'
})
export class CreateTaskDialogComponent implements OnInit {
  readonly dialogRef = inject(MatDialogRef<CreateTaskDialogComponent>);
  readonly data = inject<{ boardColumnId: string }>(MAT_DIALOG_DATA);

  private readonly fb = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly orgService = inject(OrganizationService);
  private readonly authService = inject(AuthService);
  private readonly boardStore = inject(BoardStore);

  taskForm!: FormGroup;
  members = signal<any[]>([]);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  boardColumnId: string = '';

  constructor() {
    effect(() => {
      const activeOrg = this.orgService.activeOrganization();
      if (activeOrg) {
        this.loadMembers(activeOrg.id);
      }
    });
  }

  ngOnInit(): void {
    this.boardColumnId = this.data.boardColumnId;
    this.initForm();
  }

  private initForm(): void {
    this.taskForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      priority: [1, [Validators.required]],
      assigneeId: [null],
      dueDate: [null],
      estimatedHours: [null]
    });
  }

  private loadMembers(orgId: string): void {
    this.taskService.getOrganizationMembers(orgId).subscribe({
      next: (data) => this.members.set(data),
      error: () => console.error('Failed to load organization members.')
    });
  }

  onSubmit(): void {
    if (this.taskForm.invalid) return;

    this.isSaving.set(true);
    this.errorMessage.set(null);

    const formVal = this.taskForm.value;
    const activeUser = this.authService.currentUser();
    // Default reporter to current user
    const reporterId = activeUser ? activeUser.sub : null;

    const payload = {
      boardColumnId: this.boardColumnId,
      title: formVal.title,
      description: formVal.description || '',
      priority: Number(formVal.priority),
      assigneeId: formVal.assigneeId || null,
      reporterId: reporterId || null,
      dueDate: formVal.dueDate ? new Date(formVal.dueDate).toISOString() : null,
      estimatedHours: formVal.estimatedHours !== null && formVal.estimatedHours !== '' ? Number(formVal.estimatedHours) : null
    };

    this.taskService.createTask(payload).subscribe({
      next: () => {
        this.isSaving.set(false);
        // Refresh board
        this.boardStore.loadBoard(this.boardStore.projectId());
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.Message || 'Fout bij het aanmaken van de taak.');
      }
    });
  }
}
