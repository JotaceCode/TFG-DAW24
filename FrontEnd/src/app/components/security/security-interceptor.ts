import {
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';

// For send Bearer token in each request and get permission from the API (must be logged)
export const SecurityInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
) => {
  if (typeof localStorage === 'undefined') {
    return next(req);
  }
  const tokenSeguridad = localStorage.getItem('token');
  const request = req.clone({
    headers: req.headers.set('Authorization', 'Bearer ' + tokenSeguridad),
  });

  return next(request);
};
