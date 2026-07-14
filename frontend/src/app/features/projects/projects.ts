import { Component, inject, signal, effect } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ProjectService } from '../../core/services/project.service';
import { OrganizationService } from '../../core/services/organization.service';
import { CreateProjectDialogComponent } from './create-project-dialog/create-project-dialog';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './projects.html',
  styleUrl: './projects.scss'
})
export class ProjectsComponent {
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly projectService = inject(ProjectService);
  protected readonly orgService = inject(OrganizationService);

  projects = signal<any[]>([]);

  constructor() {
    // Reload projects dynamically on organization change
    effect(() => {
      const activeOrg = this.orgService.activeOrganization();
      if (activeOrg) {
        this.loadProjects(activeOrg.id);
      } else {
        this.projects.set([]);
      }
    });
  }

  loadProjects(orgId: string): void {
    this.projectService.getProjects(orgId).subscribe({
      next: (data) => this.projects.set(data),
      error: () => this.projects.set([])
    });
  }

  openCreateProjectDialog(): void {
    const activeOrg = this.orgService.activeOrganization();
    if (!activeOrg) return;

    const dialogRef = this.dialog.open(CreateProjectDialogComponent, {
      width: '500px',
      autoFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.projectService.createProject(activeOrg.id, result).subscribe({
          next: (newProj) => {
            // Refresh grid
            this.loadProjects(activeOrg.id);
            // Navigate directly to the Kanban board of the new project
            this.router.navigate(['/projects', newProj.id, 'board']);
          }
        });
      }
    });
  }

  goToBoard(projectId: string): void {
    this.router.navigate(['/projects', projectId, 'board']);
  }

  getIconClass(icon: string): string {
    if (icon && icon.startsWith('bi-')) {
      return `bi ${icon}`;
    }
    const mapping: { [key: string]: string } = {
      'assignment': 'bi bi-clipboard-data',
      'code': 'bi bi-code-slash',
      'design_services': 'bi bi-palette',
      'bug_report': 'bi bi-bug',
      'rocket': 'bi bi-rocket-takeoff',
      'analytics': 'bi bi-bar-chart-line',
      'campaign': 'bi bi-megaphone',
      'extension': 'bi bi-puzzle'
    };
    return mapping[icon] || 'bi bi-clipboard-data';
  }
}
