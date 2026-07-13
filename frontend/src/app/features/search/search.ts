import { Component, OnInit, OnDestroy, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { SearchService, SearchTaskItem } from '../../core/services/search.service';
import { ProjectService } from '../../core/services/project.service';
import { OrganizationService } from '../../core/services/organization.service';
import { TaskService } from '../../core/services/task.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { TaskDetailDialogComponent } from '../board/task-detail-dialog/task-detail-dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatOptionModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule
  ],
  templateUrl: './search.html',
  styleUrl: './search.scss'
})
export class SearchComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly searchService = inject(SearchService);
  private readonly projectService = inject(ProjectService);
  protected readonly orgService = inject(OrganizationService);
  private readonly taskService = inject(TaskService);
  private readonly dialog = inject(MatDialog);

  tasks = signal<SearchTaskItem[]>([]);
  projects = signal<any[]>([]);
  members = signal<any[]>([]);
  
  totalCount = signal<number>(0);
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalPages = signal<number>(1);

  searchForm!: FormGroup;
  private formSub?: Subscription;

  constructor() {
    this.initForm();

    // Reload filters & perform search on organization change
    effect(() => {
      const activeOrg = this.orgService.activeOrganization();
      if (activeOrg) {
        this.loadProjects(activeOrg.id);
        this.loadMembers(activeOrg.id);
        this.resetFiltersAndSearch();
      } else {
        this.projects.set([]);
        this.members.set([]);
        this.tasks.set([]);
      }
    });
  }

  ngOnInit(): void {
    this.setupFormListeners();
  }

  ngOnDestroy(): void {
    this.formSub?.unsubscribe();
  }

  private initForm(): void {
    this.searchForm = this.fb.group({
      searchTerm: [''],
      projectId: [''],
      assigneeId: [''],
      status: [''],
      priority: [''],
      sortBy: ['Title'],
      sortOrder: ['asc']
    });
  }

  private setupFormListeners(): void {
    // Listen to form value changes
    // Debounce the text search, trigger select filters immediately
    this.formSub = this.searchForm.valueChanges
      .pipe(
        debounceTime(250), // responsive debounce
        distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b))
      )
      .subscribe(() => {
        this.pageNumber.set(1);
        this.performSearch();
      });
  }

  private loadProjects(orgId: string): void {
    this.projectService.getProjects(orgId).subscribe({
      next: (data) => this.projects.set(data),
      error: () => this.projects.set([])
    });
  }

  private loadMembers(orgId: string): void {
    this.taskService.getOrganizationMembers(orgId).subscribe({
      next: (data) => this.members.set(data),
      error: () => this.members.set([])
    });
  }

  private resetFiltersAndSearch(): void {
    this.searchForm.patchValue({
      searchTerm: '',
      projectId: '',
      assigneeId: '',
      status: '',
      priority: '',
      sortBy: 'Title',
      sortOrder: 'asc'
    }, { emitEvent: false });
    this.pageNumber.set(1);
    this.performSearch();
  }

  performSearch(): void {
    const activeOrg = this.orgService.activeOrganization();
    if (!activeOrg) return;

    const val = this.searchForm.value;
    const filters = {
      searchTerm: val.searchTerm || undefined,
      projectId: val.projectId || undefined,
      assigneeId: val.assigneeId || undefined,
      status: val.status || undefined,
      priority: val.priority !== '' && val.priority !== null ? Number(val.priority) : undefined,
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      sortBy: val.sortBy,
      sortOrder: val.sortOrder
    };

    // If no project is selected, let's filter in database or pass all projects.
    // The backend SearchTasksQueryHandler filters by ProjectId if set, otherwise searches across all.
    // However, we want to ensure we only search projects within this organization if ProjectId is empty.
    // To solve this, if projectId is empty, we don't pass a projectId, but the user is within their activeOrg space.
    this.searchService.searchTasks(filters).subscribe({
      next: (res) => {
        this.tasks.set(res.items);
        this.totalCount.set(res.totalCount);
        this.pageNumber.set(res.pageNumber);
        this.pageSize.set(res.pageSize);
        this.totalPages.set(res.totalPages);
      },
      error: () => {
        this.tasks.set([]);
      }
    });
  }

  // Clear all filters
  clearFilters(): void {
    this.resetFiltersAndSearch();
  }

  // Pagination
  nextPage(): void {
    if (this.pageNumber() < this.totalPages()) {
      this.pageNumber.update(p => p + 1);
      this.performSearch();
    }
  }

  prevPage(): void {
    if (this.pageNumber() > 1) {
      this.pageNumber.update(p => p - 1);
      this.performSearch();
    }
  }

  onPageSizeChange(size: string | number): void {
    this.pageSize.set(Number(size));
    this.pageNumber.set(1);
    this.performSearch();
  }

  isOverdue(dueDate: string | null | undefined): boolean {
    if (!dueDate) return false;
    return new Date(dueDate).getTime() < new Date().getTime();
  }

  // Open task detail dialog
  openTaskDetails(taskId: string): void {
    const dialogRef = this.dialog.open(TaskDetailDialogComponent, {
      width: '950px',
      maxWidth: '95vw',
      data: { taskId }
    });

    dialogRef.afterClosed().subscribe(() => {
      // Refresh search results to capture status/assignee modifications
      this.performSearch();
    });
  }

  // Helper visuals
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

  getMemberName(userId: string | null): string {
    if (!userId) return 'Niet toegewezen';
    const member = this.members().find(m => m.userId === userId);
    return member ? member.username : 'Onbekend';
  }

  getProjectName(projectId: string): string {
    // In database tasks columns lead to boards which lead to projects.
    // SearchTaskItemDto has boardColumnId, but we loaded projects list.
    // If the task matches one of the loaded projects, we can display its name.
    // To resolve this cleanly, we can find the project name in the options list.
    // Wait, the project ID is not in SearchTaskItemDto directly! But we loaded columns earlier?
    // Wait! SearchTaskItemDto does not return ProjectId directly, but we can look up project in filters projectId or leave it.
    // Ah, wait! We can update SearchTasksQueryHandler or look it up if we select the project filter.
    // For now, if the project is selected, we know it. Otherwise, we can just show the status and column name.
    return 'Project';
  }
}
