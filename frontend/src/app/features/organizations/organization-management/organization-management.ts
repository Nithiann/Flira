import { Component, OnInit, inject, signal, effect, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { OrganizationService } from '../../../core/services/organization.service';
import { AuthService } from '../../../core/services/auth.service';
import { ProfileService } from '../../../core/services/profile.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { debounceTime, distinctUntilChanged, switchMap, of } from 'rxjs';

@Component({
  selector: 'app-organization-management',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatAutocompleteModule
  ],
  templateUrl: './organization-management.html',
  styleUrl: './organization-management.scss'
})
export class OrganizationManagementComponent implements OnInit {
  protected readonly orgService = inject(OrganizationService);
  private readonly authService = inject(AuthService);
  private readonly profileService = inject(ProfileService);
  private readonly router = inject(Router);

  isSaving = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  orgMembers = signal<any[]>([]);
  orgName = signal<string>('');
  orgDescription = signal<string>('');

  showAddMember = signal<boolean>(false);
  newMemberRole = signal<string>('Member');
  
  // Autocomplete search control
  searchControl = new FormControl('', [Validators.required]);
  filteredUsers = signal<any[]>([]);

  isOwner = computed(() => this.orgService.currentUserRole() === 'Owner');
  isAdmin = computed(() => this.orgService.currentUserRole() === 'Admin');
  currentUserId = computed(() => this.authService.currentUser()?.sub || this.authService.currentUser()?.id || '');

  constructor() {
    // Setup reactive loading of organization details and members list
    effect(() => {
      const activeOrg = this.orgService.activeOrganization();
      if (activeOrg) {
        this.orgName.set(activeOrg.name || '');
        this.orgDescription.set(activeOrg.description || '');
        this.loadOrgMembers(activeOrg.id);
      } else {
        this.orgMembers.set([]);
        this.orgName.set('');
        this.orgDescription.set('');
      }
    });

    // Reactive autocomplete search query binding
    this.searchControl.valueChanges.pipe(
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

  ngOnInit(): void {
    // Redirect user back to dashboard if they are not Owner or Admin
    const role = this.orgService.currentUserRole();
    if (role !== 'Owner' && role !== 'Admin') {
      this.router.navigate(['/dashboard']);
    }
  }

  loadOrgMembers(orgId: string): void {
    this.orgService.getOrganizationMembers(orgId).subscribe({
      next: (data) => this.orgMembers.set(data),
      error: () => this.orgMembers.set([])
    });
  }

  saveOrgDetails(): void {
    const activeOrg = this.orgService.activeOrganization();
    if (!activeOrg || !this.orgName().trim()) return;

    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.orgService.updateOrganization(activeOrg.id, {
      name: this.orgName().trim(),
      description: this.orgDescription().trim()
    }).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Organisatie succesvol bijgewerkt.');
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.Message || 'Fout bij het bijwerken van de organisatie.');
      }
    });
  }

  deleteOrg(): void {
    const activeOrg = this.orgService.activeOrganization();
    if (!activeOrg) return;

    if (confirm(`Weet je zeker dat je de organisatie "${activeOrg.name}" wilt verwijderen? Dit kan niet ongedaan worden gemaakt en zal alle projecten en taken verwijderen!`)) {
      this.isSaving.set(true);
      this.errorMessage.set(null);
      this.successMessage.set(null);

      this.orgService.deleteOrganization(activeOrg.id).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.isSaving.set(false);
          this.errorMessage.set(err.error?.Message || 'Fout bij het verwijderen van de organisatie.');
        }
      });
    }
  }

  addOrgMember(): void {
    const activeOrg = this.orgService.activeOrganization();
    const email = this.searchControl.value;
    if (!activeOrg || !email || !email.trim()) return;

    // Strict existence check: verify email is actually in the filteredUsers search list
    const userExists = this.filteredUsers().some(u => u.email.toLowerCase() === email.toLowerCase());
    if (!userExists) {
      this.errorMessage.set('Gebruiker bestaat niet in het systeem en kan niet worden toegevoegd.');
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.orgService.addOrganizationMember(activeOrg.id, email.trim(), this.newMemberRole()).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.searchControl.setValue('');
        this.newMemberRole.set('Member');
        this.showAddMember.set(false);
        this.successMessage.set('Lid succesvol toegevoegd.');
        this.loadOrgMembers(activeOrg.id);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.Message || 'Fout bij het toevoegen van het lid.');
      }
    });
  }

  removeOrgMember(userId: string): void {
    const activeOrg = this.orgService.activeOrganization();
    if (!activeOrg) return;

    if (confirm('Weet je zeker dat je dit lid wilt verwijderen uit de organisatie?')) {
      this.isSaving.set(true);
      this.errorMessage.set(null);
      this.successMessage.set(null);

      this.orgService.removeOrganizationMember(activeOrg.id, userId).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.successMessage.set('Lid succesvol verwijderd.');
          this.loadOrgMembers(activeOrg.id);
        },
        error: (err) => {
          this.isSaving.set(false);
          this.errorMessage.set(err.error?.Message || 'Fout bij het verwijderen van het lid.');
        }
      });
    }
  }

  onMemberRoleChange(userId: string, newRole: string): void {
    const activeOrg = this.orgService.activeOrganization();
    if (!activeOrg) return;

    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.orgService.updateOrganizationMemberRole(activeOrg.id, userId, newRole).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Rol van lid succesvol bijgewerkt.');
        this.loadOrgMembers(activeOrg.id);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.Message || 'Fout bij het bijwerken van de rol.');
        this.loadOrgMembers(activeOrg.id);
      }
    });
  }
}
