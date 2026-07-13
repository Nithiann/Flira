import { Routes } from '@angular/router';
import { AuthLayoutComponent } from './core/layout/auth-layout/auth-layout';
import { MainLayoutComponent } from './core/layout/main-layout/main-layout';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Public Auth Routes
  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () => import('./features/auth/register/register').then(m => m.RegisterComponent)
      },
      {
        path: 'forgot-password',
        loadComponent: () => import('./features/auth/forgot-password/forgot-password').then(m => m.ForgotPasswordComponent)
      },
      {
        path: 'reset-password',
        loadComponent: () => import('./features/auth/reset-password/reset-password').then(m => m.ResetPasswordComponent)
      }
    ]
  },

  // Protected App Routes
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then(m => m.DashboardComponent)
      },
      {
        path: 'projects',
        loadComponent: () => import('./features/projects/projects').then(m => m.ProjectsComponent)
      },
      {
        path: 'search',
        loadComponent: () => import('./features/search/search').then(m => m.SearchComponent)
      },
      {
        path: 'projects/:projectId/board',
        loadComponent: () => import('./features/board/board').then(m => m.BoardComponent)
      },
      {
        path: 'projects/boards/:boardId/tasks/:taskId',
        loadComponent: () => import('./features/board/task-resolve.component').then(m => m.TaskResolveComponent)
      },
      {
        path: 'teams',
        loadComponent: () => import('./features/teams/team-management').then(m => m.TeamManagementComponent)
      },
      {
        path: 'settings',
        loadComponent: () => import('./features/settings/settings').then(m => m.SettingsComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./features/profile/profile').then(m => m.ProfileComponent)
      }
    ]
  },

  // Wildcard fallback
  { path: '**', redirectTo: 'dashboard' }
];
