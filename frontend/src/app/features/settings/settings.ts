import { Component } from '@angular/core';

@Component({
  selector: 'app-settings',
  standalone: true,
  template: `
    <div class="glass-card" style="padding: 2.5rem; border-radius: 16px;">
      <h1 style="margin-top: 0; font-size: 2.25rem; font-weight: 800; letter-spacing: -0.025em;">Settings</h1>
      <p style="color: var(--text-muted); font-size: 1.1rem; line-height: 1.6;">Pas hier uw profielinstellingen aan, wissel van organisatie of configureer uw notificatievoorkeuren.</p>
    </div>
  `
})
export class SettingsComponent {}
