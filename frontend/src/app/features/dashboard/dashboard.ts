import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div class="glass-card" style="padding: 2.5rem; border-radius: 16px;">
      <h1 style="margin-top: 0; font-size: 2.25rem; font-weight: 800; letter-spacing: -0.025em;">Dashboard</h1>
      <p style="color: var(--text-muted); font-size: 1.1rem; line-height: 1.6;">Welkom bij Flira! Hier verschijnen binnenkort uw projectvoortgang, burndown charts en statistieken.</p>
    </div>
  `
})
export class DashboardComponent {}
