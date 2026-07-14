import { Injectable, signal, effect } from '@angular/core';

export interface AccentOption {
  name: string;
  value: string;
  color: string;
}

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'flira-theme';
  private readonly ACCENT_TYPE_KEY = 'flira-accent-type';
  private readonly ACCENT_COLOR_KEY = 'flira-accent-color';

  readonly isDarkMode = signal<boolean>(false);
  readonly accentType = signal<string>('blue');
  readonly customAccentColor = signal<string>('#3b82f6');
  readonly accentColor = signal<string>('#3b82f6');

  readonly accentOptions: AccentOption[] = [
    { name: 'Blauw', value: 'blue', color: '#3b82f6' },
    { name: 'Groen', value: 'green', color: '#22c55e' },
    { name: 'Paars', value: 'purple', color: '#a855f7' },
    { name: 'Roze', value: 'pink', color: '#ec4899' },
    { name: 'Rood', value: 'red', color: '#ef4444' },
    { name: 'Geel', value: 'yellow', color: '#eab308' },
    { name: 'Wit', value: 'white', color: '#ffffff' }
  ];

  constructor() {
    const savedTheme = localStorage.getItem(this.THEME_KEY);
    const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    
    const initialMode = savedTheme === 'dark' || (!savedTheme && prefersDark);
    this.isDarkMode.set(initialMode);

    const savedAccentType = localStorage.getItem(this.ACCENT_TYPE_KEY) || 'blue';
    const savedAccentColor = localStorage.getItem(this.ACCENT_COLOR_KEY) || '#3b82f6';
    
    this.accentType.set(savedAccentType);
    this.customAccentColor.set(savedAccentColor);
    
    this.updateAccentColorValue(savedAccentType, savedAccentColor, initialMode);

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
      this.updateAccentColorValue(this.accentType(), this.customAccentColor(), isDark);
    });

    // Automatically apply CSS custom property when the accent color changes
    effect(() => {
      const color = this.accentColor();
      document.documentElement.style.setProperty('--accent-color', color);
    });
  }

  toggleTheme(): void {
    this.isDarkMode.update(dark => !dark);
  }

  setAccentType(type: string): void {
    this.accentType.set(type);
    localStorage.setItem(this.ACCENT_TYPE_KEY, type);
    this.updateAccentColorValue(type, this.customAccentColor(), this.isDarkMode());
  }

  setCustomAccentColor(color: string): void {
    this.customAccentColor.set(color);
    localStorage.setItem(this.ACCENT_COLOR_KEY, color);
    if (this.accentType() === 'custom') {
      this.updateAccentColorValue('custom', color, this.isDarkMode());
    }
  }

  private updateAccentColorValue(type: string, customColor: string, isDark: boolean): void {
    if (type === 'custom') {
      this.accentColor.set(customColor);
      return;
    }

    if (type === 'white') {
      this.accentColor.set(isDark ? '#ffffff' : '#0f172a');
      return;
    }

    const matched = this.accentOptions.find(o => o.value === type);
    this.accentColor.set(matched ? matched.color : '#3b82f6');
  }
}
