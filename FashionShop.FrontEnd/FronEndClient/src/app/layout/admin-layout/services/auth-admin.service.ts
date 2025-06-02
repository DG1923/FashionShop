import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap, BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthAdminService {
  private baseUrl = environment.apiUrl+"/auth";
  private isAdminSubject = new BehaviorSubject<boolean>(this.hasValidAdminToken());

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, { email, password })
      .pipe(
        tap((response: any) => {
          if (response.token) {
            localStorage.setItem('admin_token', response.token); // Make sure this key matches getAdminToken
            this.isAdminSubject.next(true);
          }
        })
      );
  }

  logout() {
    localStorage.removeItem('admin_token');
    this.isAdminSubject.next(false);
  }

  isAdmin(): boolean {
    return this.isAdminSubject.value;
  }
  getAdminToken(): string | null {
    return localStorage.getItem('admin_token');
  }

  private hasValidAdminToken(): boolean {
    const token = localStorage.getItem('admin_token');
    // Add token validation logic here
    return !!token;
  }
  getAdminHeaders() {
    const token = localStorage.getItem('admin_token'); // Make sure this matches your token storage key
    if (!token) {
      throw new Error('No authentication token found');
    }
    return {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    };
  }
}