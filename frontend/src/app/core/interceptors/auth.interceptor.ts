import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();
  const activeOrgId = localStorage.getItem('flira-active-org-id');

  let authReq = req;
  const headers: { [name: string]: string } = {};

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  if (activeOrgId) {
    headers['X-Organization-Id'] = activeOrgId;
  }

  if (Object.keys(headers).length > 0) {
    authReq = req.clone({
      setHeaders: headers
    });
  }

  return next(authReq).pipe(
    catchError((error) => {
      // Refresh token on 401 error, preventing recursive loop on refresh requests
      if (error instanceof HttpErrorResponse && error.status === 401 && !req.url.includes('/refresh')) {
        return authService.refreshToken().pipe(
          switchMap((res) => {
            const newToken = res.token;
            const retryHeaders: { [name: string]: string } = {
              Authorization: `Bearer ${newToken}`
            };
            if (activeOrgId) {
              retryHeaders['X-Organization-Id'] = activeOrgId;
            }
            const newAuthReq = req.clone({
              setHeaders: retryHeaders
            });
            return next(newAuthReq);
          }),
          catchError((refreshError) => {
            authService.logout();
            window.location.href = '/login';
            return throwError(() => refreshError);
          })
        );
      }
      return throwError(() => error);
    })
  );
};
