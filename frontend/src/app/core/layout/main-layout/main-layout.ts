import { Component, inject, signal } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { TranslateService, TranslatePipe } from '@ngx-translate/core';
import { ThemeService } from '../../services/theme.service';
import { OrganizationService } from '../../services/organization.service';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CreateOrganizationDialogComponent } from '../../../features/organizations/create-organization-dialog/create-organization-dialog';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    TranslatePipe,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatMenuModule,
    MatDialogModule
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss'
})
export class MainLayoutComponent {
  private readonly breakpointObserver = inject(BreakpointObserver);
  protected readonly themeService = inject(ThemeService);
  protected readonly translate = inject(TranslateService);
  protected readonly orgService = inject(OrganizationService);
  protected readonly notifService = inject(NotificationService);
  protected readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  readonly isMobile = signal<boolean>(false);
  readonly currentLanguage = signal<string>('nl');
  readonly isMenuOpen = signal<boolean>(false);

  constructor() {
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe(result => {
      this.isMobile.set(result.matches);
    });

    this.currentLanguage.set(this.translate.currentLang() || 'nl');
  }

  setLanguage(lang: string): void {
    this.translate.use(lang);
    this.currentLanguage.set(lang);
  }

  openCreateOrganizationDialog(): void {
    const dialogRef = this.dialog.open(CreateOrganizationDialogComponent, {
      width: '450px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.orgService.createOrganization(result).subscribe();
      }
    });
  }

  markAsRead(event: Event, notif: any): void {
    event.stopPropagation();
    this.notifService.markAsRead(notif.id).subscribe();
  }

  markAllAsRead(): void {
    this.notifService.markAllAsRead().subscribe();
  }

  handleNotificationClick(notif: any): void {
    this.notifService.markAsRead(notif.id).subscribe({
      next: () => {
        this.router.navigateByUrl(notif.link);
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getAvatarInitials(name: string): string {
    if (!name) return '??';
    return name.substring(0, 2).toUpperCase();
  }
}
