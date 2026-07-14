import { Component, OnInit, inject, signal, effect, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProjectService } from '../../core/services/project.service';
import { OrganizationService } from '../../core/services/organization.service';
import { BoardService } from '../../core/services/board.service';
import { ColumnService } from '../../core/services/column.service';
import { ThemeService } from '../../core/services/theme.service';
import { AuthService } from '../../core/services/auth.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatExpansionModule,
    MatDividerModule
  ],
  templateUrl: './settings.html',
  styleUrl: './settings.scss'
})
export class SettingsComponent implements OnInit {
  private readonly projectService = inject(ProjectService);
  protected readonly orgService = inject(OrganizationService);
  private readonly boardService = inject(BoardService);
  private readonly columnService = inject(ColumnService);
  private readonly themeService = inject(ThemeService);
  private readonly authService = inject(AuthService);

  isDarkMode = this.themeService.isDarkMode;
  selectedAccent = this.themeService.accentType;
  customAccentColor = this.themeService.customAccentColor;
  accentOptions = this.themeService.accentOptions;

  projects = signal<any[]>([]);
  selectedProjectId = signal<string>('');
  board = signal<any | null>(null);
  columns = signal<any[]>([]);

  newColumnName = '';
  editingColumnId = signal<string | null>(null);
  editingColumnName = '';

  isSaving = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  constructor() {
    // Reload projects dynamically on organization change
    effect(() => {
      const activeOrg = this.orgService.activeOrganization();
      if (activeOrg) {
        this.loadProjects(activeOrg.id);
      } else {
        this.projects.set([]);
        this.selectedProjectId.set('');
        this.board.set(null);
        this.columns.set([]);
      }
    });
  }

  ngOnInit(): void {}

  private loadProjects(orgId: string): void {
    this.projectService.getProjects(orgId).subscribe({
      next: (data) => {
        this.projects.set(data);
        if (data.length > 0) {
          // Select first project by default if none is active
          const defaultProjectId = data[0].id;
          this.selectedProjectId.set(defaultProjectId);
          this.loadBoard(defaultProjectId);
        } else {
          this.selectedProjectId.set('');
          this.board.set(null);
          this.columns.set([]);
        }
      },
      error: () => {
        this.projects.set([]);
        this.selectedProjectId.set('');
        this.board.set(null);
        this.columns.set([]);
      }
    });
  }

  onProjectSelected(projectId: string): void {
    this.selectedProjectId.set(projectId);
    this.loadBoard(projectId);
  }

  private loadBoard(projectId: string): void {
    this.errorMessage.set(null);
    this.boardService.getProjectBoard(projectId).subscribe({
      next: (projectData) => {
        const boardData = projectData.boards?.[0];
        if (boardData) {
          this.board.set(boardData);
          // Order columns by position ascending
          const sortedCols = (boardData.columns || []).sort((a: any, b: any) => a.position - b.position);
          this.columns.set(sortedCols);
        } else {
          this.board.set(null);
          this.columns.set([]);
        }
      },
      error: () => {
        this.errorMessage.set('Fout bij het laden van project kolommen.');
        this.board.set(null);
        this.columns.set([]);
      }
    });
  }

  // Add Column
  addColumn(): void {
    if (!this.newColumnName.trim() || !this.board()) return;

    this.isSaving.set(true);
    this.errorMessage.set(null);

    this.columnService.createColumn(this.board().id, this.newColumnName.trim()).subscribe({
      next: () => {
        this.newColumnName = '';
        this.loadBoard(this.selectedProjectId());
        this.isSaving.set(false);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.Message || 'Fout bij het toevoegen van de kolom.');
      }
    });
  }

  // Edit Column Name
  startEdit(col: any): void {
    this.editingColumnId.set(col.id);
    this.editingColumnName = col.name;
  }

  saveEdit(colId: string): void {
    if (!this.editingColumnName.trim()) return;

    this.isSaving.set(true);
    this.errorMessage.set(null);

    this.columnService.updateColumn(colId, this.editingColumnName.trim()).subscribe({
      next: () => {
        this.editingColumnId.set(null);
        this.loadBoard(this.selectedProjectId());
        this.isSaving.set(false);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.Message || 'Fout bij hernoemen van kolom.');
      }
    });
  }

  cancelEdit(): void {
    this.editingColumnId.set(null);
    this.editingColumnName = '';
  }

  // Delete Column
  deleteColumn(colId: string): void {
    if (confirm('Weet je zeker dat je deze kolom/status wilt verwijderen? Eventuele taken in deze kolom kunnen verloren gaan.')) {
      this.isSaving.set(true);
      this.errorMessage.set(null);

      this.columnService.deleteColumn(colId).subscribe({
        next: () => {
          this.loadBoard(this.selectedProjectId());
          this.isSaving.set(false);
        },
        error: (err) => {
          this.isSaving.set(false);
          this.errorMessage.set(err.error?.Message || 'Fout bij het verwijderen van kolom.');
        }
      });
    }
  }

  // Move Column Up/Down
  moveColumn(colId: string, currentIndex: number, direction: 'up' | 'down'): void {
    const targetIndex = direction === 'up' ? currentIndex - 1 : currentIndex + 1;
    if (targetIndex < 0 || targetIndex >= this.columns().length) return;

    this.isSaving.set(true);
    this.errorMessage.set(null);

    // Call move column API endpoint
    this.columnService.moveColumn(colId, targetIndex).subscribe({
      next: () => {
        this.loadBoard(this.selectedProjectId());
        this.isSaving.set(false);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.Message || 'Fout bij verplaatsen van kolom.');
      }
    });
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  selectAccent(type: string): void {
    this.themeService.setAccentType(type);
  }

  onCustomColorInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input) {
      this.themeService.setAccentType('custom');
      this.themeService.setCustomAccentColor(input.value);
    }
  }
}
