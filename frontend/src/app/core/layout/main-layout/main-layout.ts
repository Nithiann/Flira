import { Component, inject, signal } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslateService, TranslatePipe } from '@ngx-translate/core';
import { ThemeService } from '../../services/theme.service';
import { OrganizationService } from '../../services/organization.service';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { CommonModule } from '@angular/common';

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
    MatMenuModule
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss'
})
export class MainLayoutComponent {
  private readonly breakpointObserver = inject(BreakpointObserver);
  protected readonly themeService = inject(ThemeService);
  protected readonly translate = inject(TranslateService);
  protected readonly orgService = inject(OrganizationService);

  readonly isMobile = signal<boolean>(false);
  readonly currentLanguage = signal<string>('nl');

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

  logout(): void {
    // Will be wired to AuthService later
    console.log('Logging out...');
    localStorage.removeItem('flira-token');
    localStorage.removeItem('flira-refresh-token');
    window.location.href = '/login';
  }
}
