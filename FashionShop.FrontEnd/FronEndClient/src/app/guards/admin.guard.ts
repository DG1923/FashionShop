import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthAdminService } from '../layout/admin-layout/services/auth-admin.service';
import { HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(
    private authAdminService: AuthAdminService,
    private router: Router
  ) {}
    getAdminHeaders(): HttpHeaders {
    const token = this.authAdminService.getAdminToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  canActivate(): boolean {
    if (this.authAdminService.isAdmin()) {
      return true;
    }
    
    this.router.navigate(['/admin/login']);
    return false;
  }
}