import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-create-project-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './create-project-dialog.html',
  styleUrl: './create-project-dialog.scss'
})
export class CreateProjectDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<CreateProjectDialogComponent>);

  projectForm: FormGroup;

  // Premium predefined colors
  readonly colors = [
    '#3b82f6', // Blue
    '#8b5cf6', // Purple
    '#ec4899', // Pink
    '#ef4444', // Red
    '#f97316', // Orange
    '#10b981', // Green
    '#14b8a6', // Teal
    '#64748b'  // Slate
  ];

  // Predefined icons
  readonly icons = [
    'bi-clipboard-data',
    'bi-code-slash',
    'bi-palette',
    'bi-bug',
    'bi-rocket-takeoff',
    'bi-bar-chart-line',
    'bi-megaphone',
    'bi-puzzle'
  ];

  selectedColor = signal<string>('#3b82f6');
  selectedIcon = signal<string>('bi-clipboard-data');

  constructor() {
    this.projectForm = this.fb.nonNullable.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]]
    });
  }

  getIconClass(icon: string): string {
    return `bi ${icon}`;
  }

  selectColor(color: string): void {
    this.selectedColor.set(color);
  }

  selectIcon(icon: string): void {
    this.selectedIcon.set(icon);
  }

  onSubmit(): void {
    if (this.projectForm.invalid) return;

    const result = {
      ...this.projectForm.getRawValue(),
      color: this.selectedColor(),
      icon: this.selectedIcon()
    };

    this.dialogRef.close(result);
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
