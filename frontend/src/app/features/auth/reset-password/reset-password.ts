import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';
import { passwordMatchValidator } from '../register/register';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    TranslatePipe
  ],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss'
})
export class ResetPasswordComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);

  resetForm: FormGroup;
  hidePassword = signal<boolean>(true);
  isLoading = signal<boolean>(false);
  isSuccess = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  token: string = '';

  constructor() {
    this.token = this.route.snapshot.queryParams['token'] || '';

    this.resetForm = this.fb.nonNullable.group({
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: passwordMatchValidator });
  }

  onSubmit(): void {
    if (this.resetForm.invalid || !this.token) {
      if (!this.token) {
        this.errorMessage.set('Ongeldige of ontbrekende token. Vraag een nieuwe herstellink aan.');
      }
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const { password, confirmPassword } = this.resetForm.getRawValue();

    this.authService.resetPassword({ token: this.token, password, confirmPassword }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.isSuccess.set(true);
      },
      error: (err) => {
        this.isLoading.set(false);
        const msg = err.error?.Message || 'Fout bij het resetten van uw wachtwoord. De link is mogelijk verlopen.';
        this.errorMessage.set(msg);
      }
    });
  }
}
