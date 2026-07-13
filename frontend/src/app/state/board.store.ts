import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { inject } from '@angular/core';
import { BoardService } from '../core/services/board.service';
import { TaskService } from '../core/services/task.service';
import { AuthService } from '../core/services/auth.service';

interface ColumnWithTasks {
  id: string;
  name: string;
  position: number;
  tasks: any[];
}

interface BoardState {
  projectId: string;
  projectName: string;
  columns: ColumnWithTasks[];
  isLoading: boolean;
  errorMessage: string | null;
}

const initialState: BoardState = {
  projectId: '',
  projectName: '',
  columns: [],
  isLoading: false,
  errorMessage: null
};

export const BoardStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((
    store,
    boardService = inject(BoardService),
    taskService = inject(TaskService),
    authService = inject(AuthService)
  ) => ({
    loadBoard(projectId: string): void {
      patchState(store, { projectId, isLoading: true, errorMessage: null });

      boardService.getProjectBoard(projectId).subscribe({
        next: (projectData) => {
          const board = projectData.boards?.[0];
          if (!board || !board.columns) {
            patchState(store, {
              projectName: projectData.name,
              columns: [],
              isLoading: false
            });
            return;
          }

          const cols: ColumnWithTasks[] = board.columns.map((c: any) => ({
            id: c.id,
            name: c.name,
            position: c.position,
            tasks: []
          }));

          boardService.getProjectTasks(projectId).subscribe({
            next: (taskData) => {
              const taskItems = taskData.items || [];
              cols.forEach(col => {
                col.tasks = taskItems.filter((t: any) => t.boardColumnId === col.id);
              });

              patchState(store, {
                projectName: projectData.name,
                columns: cols.sort((a, b) => a.position - b.position),
                isLoading: false
              });
            },
            error: () => {
              patchState(store, {
                isLoading: false,
                errorMessage: 'Fout bij het laden van taken.'
              });
            }
          });
        },
        error: () => {
          patchState(store, {
            isLoading: false,
            errorMessage: 'Fout bij het laden van het bord.'
          });
        }
      });
    },

    updateTaskStatusOptimistic(taskId: string, newStatus: string, newColumnId: string): void {
      const originalColumns = JSON.parse(JSON.stringify(store.columns())) as ColumnWithTasks[];
      const cols = JSON.parse(JSON.stringify(store.columns())) as ColumnWithTasks[];

      // Visually transfer item immediately
      let taskToMove: any = null;
      for (const col of cols) {
        const idx = col.tasks.findIndex(t => t.id === taskId);
        if (idx !== -1) {
          taskToMove = col.tasks.splice(idx, 1)[0];
          break;
        }
      }

      if (!taskToMove) return;

      taskToMove.status = newStatus;
      taskToMove.boardColumnId = newColumnId;

      // Auto-assign to current user if dragging to In Progress and unassigned
      const isMovingToInProgress = newStatus.toLowerCase() === 'in progress';
      const isUnassigned = !taskToMove.assigneeId;

      if (isMovingToInProgress && isUnassigned) {
        const currentUser = authService.currentUser();
        if (currentUser && currentUser.sub) {
          taskToMove.assigneeId = currentUser.sub;
          taskToMove.assigneeName = currentUser.unique_name || currentUser.name || 'Me';
          
          const updateModel = {
            title: taskToMove.title,
            description: taskToMove.description || '',
            priority: Number(taskToMove.priority),
            assigneeId: currentUser.sub,
            reporterId: taskToMove.reporterId || null,
            dueDate: taskToMove.dueDate ? new Date(taskToMove.dueDate).toISOString() : null,
            estimatedHours: taskToMove.estimatedHours !== null && taskToMove.estimatedHours !== '' ? Number(taskToMove.estimatedHours) : null
          };

          taskService.updateTask(taskId, updateModel).subscribe({
            error: (err) => console.error('Failed to auto-assign task on drag:', err)
          });
        }
      }

      const targetCol = cols.find(c => c.id === newColumnId);
      if (targetCol) {
        targetCol.tasks.push(taskToMove);
      }

      // Apply optimistic update
      patchState(store, { columns: cols });

      // Call API
      boardService.updateTaskStatus(taskId, newStatus, newColumnId).subscribe({
        error: (err) => {
          // Rollback on failure
          patchState(store, {
            columns: originalColumns,
            errorMessage: err.error?.Message || 'Fout bij het verplaatsen van taak.'
          });
          setTimeout(() => patchState(store, { errorMessage: null }), 5000);
        }
      });
    },

    receiveTaskStatusUpdate(taskId: string, newStatus: string, newColumnId: string): void {
      const cols = JSON.parse(JSON.stringify(store.columns())) as ColumnWithTasks[];

      // Find if task is currently on the board
      let taskToMove: any = null;
      for (const col of cols) {
        const idx = col.tasks.findIndex(t => t.id === taskId);
        if (idx !== -1) {
          taskToMove = col.tasks.splice(idx, 1)[0];
          break;
        }
      }

      // If the task exists on this board, move it
      if (taskToMove) {
        taskToMove.status = newStatus;
        taskToMove.boardColumnId = newColumnId;

        const targetCol = cols.find(c => c.id === newColumnId);
        if (targetCol) {
          targetCol.tasks.push(taskToMove);
        }

        patchState(store, { columns: cols });
      }
    },

    // Refresh a single task's visual properties on the board (e.g. after detail updates)
    updateTaskDetailsOnBoard(updatedTask: any): void {
      const cols = JSON.parse(JSON.stringify(store.columns())) as ColumnWithTasks[];
      let updated = false;

      for (const col of cols) {
        const idx = col.tasks.findIndex(t => t.id === updatedTask.id);
        if (idx !== -1) {
          col.tasks[idx] = {
            ...col.tasks[idx],
            title: updatedTask.title,
            description: updatedTask.description,
            priority: updatedTask.priority,
            assigneeId: updatedTask.assigneeId,
            assigneeName: updatedTask.assigneeName // In case assignee details changed
          };
          updated = true;
          break;
        }
      }

      if (updated) {
        patchState(store, { columns: cols });
      }
    }
  }))
);
