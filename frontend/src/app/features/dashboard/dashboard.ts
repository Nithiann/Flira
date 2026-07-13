import { Component, OnInit, OnDestroy, inject, signal, effect, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectService } from '../../core/services/project.service';
import { OrganizationService } from '../../core/services/organization.service';
import { DashboardService, DashboardStats } from '../../core/services/dashboard.service';
import { ThemeService } from '../../core/services/theme.service';
import { Chart } from 'chart.js/auto';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent implements OnInit, OnDestroy {
  private readonly projectService = inject(ProjectService);
  protected readonly orgService = inject(OrganizationService);
  private readonly dashboardService = inject(DashboardService);
  protected readonly themeService = inject(ThemeService);

  projects = signal<any[]>([]);
  selectedProjectId = signal<string>('');
  stats = signal<DashboardStats | null>(null);
  errorMessage = signal<string | null>(null);

  @ViewChild('burndownCanvas') burndownCanvas!: ElementRef<HTMLCanvasElement>;
  private chart: Chart | null = null;

  constructor() {
    // Monitor active organization changes
    effect(() => {
      const activeOrg = this.orgService.activeOrganization();
      if (activeOrg) {
        this.loadProjects(activeOrg.id);
      } else {
        this.projects.set([]);
        this.selectedProjectId.set('');
        this.stats.set(null);
      }
    });

    // Monitor theme changes to update chart styling in real-time
    effect(() => {
      const isDark = this.themeService.isDarkMode();
      if (this.chart) {
        this.applyChartTheme(isDark);
      }
    });
  }

  ngOnInit(): void {}

  ngOnDestroy(): void {
    if (this.chart) {
      this.chart.destroy();
    }
  }

  private loadProjects(orgId: string): void {
    this.projectService.getProjects(orgId).subscribe({
      next: (data) => {
        this.projects.set(data);
        if (data.length > 0) {
          // Select first project by default
          const defaultProjectId = data[0].id;
          this.selectedProjectId.set(defaultProjectId);
          this.loadStats(defaultProjectId);
        } else {
          this.selectedProjectId.set('');
          this.stats.set(null);
        }
      },
      error: () => {
        this.projects.set([]);
        this.selectedProjectId.set('');
        this.stats.set(null);
      }
    });
  }

  onProjectSelected(projectId: string): void {
    this.selectedProjectId.set(projectId);
    this.loadStats(projectId);
  }

  private loadStats(projectId: string): void {
    this.errorMessage.set(null);
    this.dashboardService.getStats(projectId).subscribe({
      next: (statsData) => {
        this.stats.set(statsData);
        // Defer chart loading until DOM catches up with canvas view
        setTimeout(() => this.renderBurndownChart(statsData), 0);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.Message || 'Fout bij het laden van dashboard statistieken.');
        this.stats.set(null);
      }
    });
  }

  private renderBurndownChart(statsData: DashboardStats): void {
    if (this.chart) {
      this.chart.destroy();
    }

    if (!this.burndownCanvas) return;

    const ctx = this.burndownCanvas.nativeElement.getContext('2d');
    if (!ctx) return;

    const labels = statsData.burndownData.map(d => {
      const dateObj = new Date(d.date);
      return dateObj.toLocaleDateString('nl-NL', { day: 'numeric', month: 'short' });
    });

    const actualData = statsData.burndownData.map(d => d.remainingTasksCount);

    // Calculate Ideal Burndown Line
    // Starts at first day's remaining task count, drops linearly to 0 at the end (day 30)
    const initialRemaining = statsData.burndownData[0]?.remainingTasksCount || 0;
    const idealData = statsData.burndownData.map((_, i) => {
      const ideal = initialRemaining - (initialRemaining / 29) * i;
      return Math.max(0, parseFloat(ideal.toFixed(1)));
    });

    const isDark = this.themeService.isDarkMode();

    this.chart = new Chart(ctx, {
      type: 'line',
      data: {
        labels: labels,
        datasets: [
          {
            label: 'Werkelijke Open Taken',
            data: actualData,
            borderColor: '#3b82f6',
            backgroundColor: 'rgba(59, 130, 246, 0.08)',
            borderWidth: 3,
            tension: 0.2,
            fill: true,
            pointBackgroundColor: '#3b82f6',
            pointRadius: 4,
            pointHoverRadius: 6
          },
          {
            label: 'Ideaal Verloop',
            data: idealData,
            borderColor: '#94a3b8',
            borderDash: [6, 6],
            borderWidth: 2,
            tension: 0,
            fill: false,
            pointRadius: 0
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'top',
            labels: {
              font: { family: 'inherit', size: 12 },
              color: isDark ? '#94a3b8' : '#475569'
            }
          },
          tooltip: {
            mode: 'index',
            intersect: false
          }
        },
        scales: {
          x: {
            grid: {
              color: isDark ? 'rgba(255, 255, 255, 0.08)' : 'rgba(0, 0, 0, 0.04)'
            },
            ticks: {
              color: isDark ? '#94a3b8' : '#475569',
              font: { size: 11 }
            }
          },
          y: {
            beginAtZero: true,
            grid: {
              color: isDark ? 'rgba(255, 255, 255, 0.08)' : 'rgba(0, 0, 0, 0.04)'
            },
            ticks: {
              color: isDark ? '#94a3b8' : '#475569',
              font: { size: 11 },
              stepSize: 1
            }
          }
        }
      }
    });
  }

  private applyChartTheme(isDark: boolean): void {
    if (!this.chart || !this.chart.options.scales) return;

    const textColor = isDark ? '#94a3b8' : '#475569';
    const gridColor = isDark ? 'rgba(255, 255, 255, 0.08)' : 'rgba(0, 0, 0, 0.04)';

    // Update scale ticks and grids
    if (this.chart.options.scales['x']) {
      this.chart.options.scales['x'].ticks!.color = textColor;
      this.chart.options.scales['x'].grid!.color = gridColor;
    }
    if (this.chart.options.scales['y']) {
      this.chart.options.scales['y'].ticks!.color = textColor;
      this.chart.options.scales['y'].grid!.color = gridColor;
    }

    // Update legend labels
    if (this.chart.options.plugins?.legend?.labels) {
      this.chart.options.plugins.legend.labels.color = textColor;
    }

    this.chart.update();
  }
}
