import { Component, inject, signal, effect } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TeamService } from '../../core/services/team.service';
import { OrganizationService } from '../../core/services/organization.service';
import { ProfileService } from '../../core/services/profile.service';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { debounceTime, distinctUntilChanged, switchMap, of } from 'rxjs';

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
    MatDividerModule,
    MatAutocompleteModule
  ],
  templateUrl: './team-management.html',
  styleUrl: './team-management.scss'
})
export class TeamManagementComponent {
  private readonly fb = inject(FormBuilder);
  private readonly teamService = inject(TeamService);
  protected readonly orgService = inject(OrganizationService);
  private readonly profileService = inject(ProfileService);

  teams = signal<any[]>([]);
  selectedTeam = signal<any | null>(null);
  members = signal<any[]>([]);
  filteredUsers = signal<any[]>([]);
  
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

    // Reactive user search autocomplete
    this.inviteForm.get('email')?.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(value => {
        const val = typeof value === 'string' ? value : '';
        if (!val || val.length < 2) {
          return of([]);
        }
        return this.profileService.searchUsers(val);
      })
    ).subscribe({
      next: (users) => this.filteredUsers.set(users),
      error: () => this.filteredUsers.set([])
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

    const email = this.inviteForm.get('email')?.value;
    const userExists = this.filteredUsers().some(u => u.email.toLowerCase() === email.toLowerCase());
    if (!userExists) {
      this.errorMessage.set('Gebruiker bestaat niet en kan niet worden toegevoegd.');
      return;
    }

    this.isLoading.set(true);
    this.successMessage.set(null);
    this.errorMessage.set(null);

    const data = this.inviteForm.getRawValue();
    this.teamService.inviteMember(team.id, data).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.inviteForm.reset({ email: '', role: 'User' });
        this.showInviteForm.set(false);
        this.successMessage.set('Lid succesvol toegevoegd aan het team!');
        // Refresh members
        this.selectTeam(team);
      },
      error: (err) => {
        this.isLoading.set(false);
        const msg = err.error?.Message || 'Fout bij het toevoegen van lid.';
        this.errorMessage.set(msg);
      }
    });
  }
}
