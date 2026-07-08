import { Injectable, signal, effect } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'flira-theme';
  readonly isDarkMode = signal<boolean>(false);

  constructor() {
    const savedTheme = localStorage.getItem(this.THEME_KEY);
    const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    
    const initialMode = savedTheme === 'dark' || (!savedTheme && prefersDark);
    this.isDarkMode.set(initialMode);

    // Automatically sync document class and localStorage when the signal changes
    effect(() => {
      const isDark = this.isDarkMode();
      if (isDark) {
        document.documentElement.classList.add('dark-theme');
        localStorage.setItem(this.THEME_KEY, 'dark');
      } else {
        document.documentElement.classList.remove('dark-theme');
        localStorage.setItem(this.THEME_KEY, 'light');
      }
    });
  }

  toggleTheme(): void {
    this.isDarkMode.update(dark => !dark);
  }
}
