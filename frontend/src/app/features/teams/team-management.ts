import { Component, inject, signal, effect } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TeamService } from '../../core/services/team.service';
import { OrganizationService } from '../../core/services/organization.service';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-team-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatListModule,
    MatDividerModule
  ],
  templateUrl: './team-management.html',
  styleUrl: './team-management.scss'
})
export class TeamManagementComponent {
  private readonly fb = inject(FormBuilder);
  private readonly teamService = inject(TeamService);
  protected readonly orgService = inject(OrganizationService);

  teams = signal<any[]>([]);
  selectedTeam = signal<any | null>(null);
  members = signal<any[]>([]);
  
  createTeamForm: FormGroup;
  inviteForm: FormGroup;
  
  showCreateTeam = signal<boolean>(false);
  showInviteForm = signal<boolean>(false);
  isLoading = signal<boolean>(false);
  successMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  constructor() {
    this.createTeamForm = this.fb.nonNullable.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['']
    });

    this.inviteForm = this.fb.nonNullable.group({
      email: ['', [Validators.required, Validators.email]],
      role: ['User', [Validators.required]]
    });

    // Reactive load of teams on active organization change
    effect(() => {
      const activeOrg = this.orgService.activeOrganization();
      if (activeOrg) {
        this.loadTeams(activeOrg.id);
        this.selectedTeam.set(null);
        this.members.set([]);
      }
    });
  }

  loadTeams(orgId: string): void {
    this.teamService.getTeams(orgId).subscribe({
      next: (data) => this.teams.set(data),
      error: () => this.teams.set([])
    });
  }

  selectTeam(team: any): void {
    this.selectedTeam.set(team);
    this.showInviteForm.set(false);
    this.successMessage.set(null);
    this.errorMessage.set(null);
    
    this.teamService.getTeamMembers(team.id).subscribe({
      next: (data) => this.members.set(data),
      error: () => this.members.set([])
    });
  }

  onCreateTeam(): void {
    const activeOrg = this.orgService.activeOrganization();
    if (this.createTeamForm.invalid || !activeOrg) return;

    const data = this.createTeamForm.getRawValue();
    this.teamService.createTeam(activeOrg.id, data).subscribe({
      next: () => {
        this.createTeamForm.reset();
        this.showCreateTeam.set(false);
        this.loadTeams(activeOrg.id);
      }
    });
  }

  onInviteMember(): void {
    const team = this.selectedTeam();
    if (this.inviteForm.invalid || !team) return;

    this.isLoading.set(true);
    this.successMessage.set(null);
    this.errorMessage.set(null);

    const data = this.inviteForm.getRawValue();
    this.teamService.inviteMember(team.id, data).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.inviteForm.reset({ email: '', role: 'User' });
        this.showInviteForm.set(false);
        this.successMessage.set('Uitnodiging succesvol verzonden!');
        // Refresh members
        this.selectTeam(team);
      },
      error: (err) => {
        this.isLoading.set(false);
        const msg = err.error?.Message || 'Fout bij het uitnodigen van lid.';
        this.errorMessage.set(msg);
      }
    });
  }
}
