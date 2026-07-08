import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(authReq).pipe(
    catchError((error) => {
      // Refresh token on 401 error, preventing recursive loop on refresh requests
      if (error instanceof HttpErrorResponse && error.status === 401 && !req.url.includes('/refresh')) {
        return authService.refreshToken().pipe(
          switchMap((res) => {
            const newToken = res.token;
            const newAuthReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${newToken}`
              }
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
