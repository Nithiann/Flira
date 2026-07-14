import { Component, Inject, OnInit, inject, signal, effect, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TaskService } from '../../../core/services/task.service';
import { OrganizationService } from '../../../core/services/organization.service';
import { BoardStore } from '../../../state/board.store';
import { MarkdownPipe } from '../../../shared/pipes/markdown.pipe';
import { HttpEvent, HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-task-detail-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatOptionModule,
    MatTooltipModule,
    MatProgressBarModule,
    MarkdownPipe
  ],
  templateUrl: './task-detail-dialog.html',
  styleUrl: './task-detail-dialog.scss'
})
export class TaskDetailDialogComponent implements OnInit {
  readonly dialogRef = inject(MatDialogRef<TaskDetailDialogComponent>);
  readonly data = inject<{ taskId: string }>(MAT_DIALOG_DATA);

  private readonly fb = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly orgService = inject(OrganizationService);
  private readonly boardStore = inject(BoardStore);

  taskId: string = '';
  task: any = null;
  comments = signal<any[]>([]);
  members = signal<any[]>([]);
  attachments = signal<any[]>([]);

  isLoading = signal<boolean>(true);
  isSaving = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  // Inline editing states
  isEditingTitle = signal<boolean>(false);
  isEditingDesc = signal<boolean>(false);

  // File upload state
  uploadProgress = signal<number | null>(null);
  uploadingFileName = signal<string>('');
  isDragging = signal<boolean>(false);

  // Autocomplete mentions state
  showMentions = signal<boolean>(false);
  mentionFilter = signal<string>('');
  mentionIndex = -1; // Cursor index where @ was typed
  @ViewChild('commentInput') commentInputRef!: ElementRef<HTMLTextAreaElement>;

  // Forms
  taskForm!: FormGroup;
  commentForm!: FormGroup;

  constructor() {
    effect(() => {
      const activeOrg = this.orgService.activeOrganization();
      if (activeOrg) {
        this.loadMembers(activeOrg.id);
      }
    });
  }

  ngOnInit(): void {
    this.taskId = this.data.taskId;
    this.initForms();
    this.loadTaskDetails();
    this.loadComments();
  }

  private initForms(): void {
    this.taskForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      priority: [1, [Validators.required]],
      assigneeId: [null],
      reporterId: [null],
      dueDate: [null],
      estimatedHours: [null]
    });

    this.commentForm = this.fb.group({
      content: ['', [Validators.required]]
    });
  }

  loadTaskDetails(): void {
    this.taskService.getTask(this.taskId).subscribe({
      next: (taskData) => {
        this.task = taskData;
        this.taskForm.patchValue({
          title: taskData.title,
          description: taskData.description,
          priority: taskData.priority,
          assigneeId: taskData.assigneeId,
          reporterId: taskData.reporterId,
          dueDate: taskData.dueDate ? taskData.dueDate.substring(0, 10) : null,
          estimatedHours: taskData.estimatedHours
        });
        this.attachments.set(taskData.attachments || []);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Fout bij het laden van taakdetails.');
        this.isLoading.set(false);
      }
    });
  }

  loadComments(): void {
    this.taskService.getComments(this.taskId).subscribe({
      next: (data) => this.comments.set(data),
      error: () => console.error('Failed to load comments.')
    });
  }

  loadMembers(orgId: string): void {
    this.taskService.getOrganizationMembers(orgId).subscribe({
      next: (data) => this.members.set(data),
      error: () => console.error('Failed to load organization members.')
    });
  }

  // Save changes
  saveTaskField(): void {
    if (this.taskForm.invalid || !this.task) return;

    const formVal = this.taskForm.value;
    const updateModel = {
      title: formVal.title,
      description: formVal.description || '',
      priority: Number(formVal.priority),
      assigneeId: formVal.assigneeId || null,
      reporterId: formVal.reporterId || null,
      dueDate: formVal.dueDate ? new Date(formVal.dueDate).toISOString() : null,
      estimatedHours: formVal.estimatedHours !== null && formVal.estimatedHours !== '' ? Number(formVal.estimatedHours) : null
    };

    this.isSaving.set(true);
    this.taskService.updateTask(this.taskId, updateModel).subscribe({
      next: () => {
        this.isSaving.set(false);
        // Refresh local task cache
        this.task = { ...this.task, ...updateModel };
        
        // Find assignee name
        const assignee = this.members().find(m => m.userId === updateModel.assigneeId);
        const assigneeName = assignee ? assignee.username : null;

        // Sync change back to Board Component Store
        this.boardStore.updateTaskDetailsOnBoard({
          id: this.taskId,
          title: updateModel.title,
          description: updateModel.description,
          priority: updateModel.priority,
          assigneeId: updateModel.assigneeId,
          assigneeName
        });
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.Message || 'Fout bij het opslaan van taak.');
        setTimeout(() => this.errorMessage.set(null), 5000);
      }
    });
  }

  onFieldChange(): void {
    // Dropdowns trigger save immediately
    this.saveTaskField();
  }

  // Delete Task
  deleteTask(): void {
    if (confirm('Weet je zeker dat je deze taak wilt verwijderen?')) {
      this.taskService.deleteTask(this.taskId).subscribe({
        next: () => {
          this.boardStore.loadBoard(this.boardStore.projectId());
          this.dialogRef.close();
        },
        error: () => alert('Fout bij het verwijderen van de taak.')
      });
    }
  }

  // Title Editing toggles
  startTitleEdit(): void {
    this.isEditingTitle.set(true);
  }

  stopTitleEdit(): void {
    this.isEditingTitle.set(false);
    this.saveTaskField();
  }

  // Description Editing toggles
  startDescEdit(): void {
    this.isEditingDesc.set(true);
  }

  stopDescEdit(): void {
    this.isEditingDesc.set(false);
    this.saveTaskField();
  }

  // Markdown Comments
  postComment(): void {
    if (this.commentForm.invalid) return;

    const content = this.commentForm.value.content;
    this.taskForm.disable(); // visual block
    
    this.taskService.addComment(this.taskId, content).subscribe({
      next: () => {
        this.commentForm.reset();
        this.loadComments();
        this.taskForm.enable();
      },
      error: () => {
        alert('Fout bij het plaatsen van reactie.');
        this.taskForm.enable();
      }
    });
  }

  // Autocomplete Mentions
  onCommentInput(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    const text = textarea.value;
    const cursor = textarea.selectionStart;

    // Check if user typed '@'
    const lastAtIdx = text.lastIndexOf('@', cursor - 1);
    if (lastAtIdx !== -1 && (lastAtIdx === 0 || /\s/.test(text[lastAtIdx - 1]))) {
      const searchStr = text.slice(lastAtIdx + 1, cursor);
      if (!/\s/.test(searchStr)) {
        this.showMentions.set(true);
        this.mentionFilter.set(searchStr);
        this.mentionIndex = lastAtIdx;
        return;
      }
    }
    this.showMentions.set(false);
  }

  getFilteredMembers(): any[] {
    const filter = this.mentionFilter().toLowerCase();
    return this.members().filter(m => 
      m.username.toLowerCase().includes(filter) || 
      m.email.toLowerCase().includes(filter)
    );
  }

  insertMention(member: any): void {
    const textarea = this.commentInputRef.nativeElement;
    const text = textarea.value;
    const cursor = textarea.selectionStart;

    const before = text.slice(0, this.mentionIndex);
    const after = text.slice(cursor);
    const mentionText = `@${member.username} `;

    this.commentForm.patchValue({
      content: before + mentionText + after
    });

    this.showMentions.set(false);
    
    // Refocus textarea and place cursor after mention
    setTimeout(() => {
      textarea.focus();
      const newCursorPos = this.mentionIndex + mentionText.length;
      textarea.setSelectionRange(newCursorPos, newCursorPos);
    });
  }

  // File Upload
  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragging.set(true);
  }

  onDragLeave(): void {
    this.isDragging.set(false);
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragging.set(false);
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.handleFileUpload(files[0]);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = input.files;
    if (files && files.length > 0) {
      this.handleFileUpload(files[0]);
    }
  }

  private handleFileUpload(file: File): void {
    // 10MB limit check
    if (file.size > 10 * 1024 * 1024) {
      alert('Bestand is te groot. Maximale bestandsgrootte is 10MB.');
      return;
    }

    this.uploadingFileName.set(file.name);
    this.uploadProgress.set(10); // start indicator

    this.taskService.uploadAttachment(this.taskId, file).subscribe({
      next: (res) => {
        // Animate completion
        this.uploadProgress.set(100);
        setTimeout(() => {
          this.uploadProgress.set(null);
          this.uploadingFileName.set('');
          this.attachments.update(list => [...list, res]);
        }, 800);
      },
      error: (err) => {
        this.uploadProgress.set(null);
        this.uploadingFileName.set('');
        alert(err.error?.Message || 'Fout bij uploaden van bestand.');
      }
    });
  }

  deleteAttachment(attachmentId: string): void {
    if (confirm('Weet je zeker dat je deze bijlage wilt verwijderen?')) {
      this.taskService.deleteAttachment(attachmentId).subscribe({
        next: () => {
          this.attachments.update(list => list.filter(a => a.id !== attachmentId));
        },
        error: () => alert('Fout bij het verwijderen van bijlage.')
      });
    }
  }

  // Helper formats
  getFileIcon(fileName: string): string {
    const ext = fileName.split('.').pop()?.toLowerCase();
    switch (ext) {
      case 'pdf': return 'bi-file-earmark-pdf text-red-500';
      case 'png':
      case 'jpg':
      case 'jpeg':
      case 'gif': return 'bi-file-earmark-image text-blue-500';
      case 'doc':
      case 'docx': return 'bi-file-earmark-word text-blue-700';
      case 'xls':
      case 'xlsx': return 'bi-file-earmark-excel text-green-600';
      case 'zip':
      case 'rar': return 'bi-file-zip text-yellow-600';
      default: return 'bi-file-earmark text-gray-500';
    }
  }

  isImage(fileName: string): boolean {
    const ext = fileName.split('.').pop()?.toLowerCase() || '';
    return ['png', 'jpg', 'jpeg', 'gif', 'webp'].includes(ext);
  }

  getDownloadUrl(fileUrl: string): string {
    if (fileUrl.startsWith('/uploads')) {
      return `http://localhost:8080${fileUrl}`;
    }
    return fileUrl;
  }

  formatBytes(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
  }

  getPriorityLabel(priority: number): string {
    switch (priority) {
      case 3: return 'Critical';
      case 2: return 'Hoog';
      case 1: return 'Medium';
      default: return 'Laag';
    }
  }

  getMemberName(userId: string | null): string {
    if (!userId) return 'Niet toegewezen';
    const member = this.members().find(m => m.userId === userId);
    return member ? member.username : 'Niet toegewezen';
  }

  getReporterName(userId: string | null): string {
    if (!userId) return 'Geen reporter';
    const member = this.members().find(m => m.userId === userId);
    return member ? member.username : 'Geen reporter';
  }
}
