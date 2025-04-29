import { HttpInterceptorFn } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AuthService } from '../services/auth.service';
export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();
  if(token){
    const cloneReq = req.clone({
      headers:req.headers.set('Authorization',`Bearer ${token}`)
    })
    return next(cloneReq);
  }
  return next(req);
};
