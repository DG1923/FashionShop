import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthAdminService } from './auth-admin.service';

export interface User {
  id: string;
  userName: string;
  email: string;
}

export interface UpdateUserDto {
  userName?: string;
  email?: string;
  password?: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserAdminService {
  private baseUrl = environment.apiUrl+"/users";

  constructor(
    private http: HttpClient,
    private authAdminService: AuthAdminService
  ) {}

  getUserById(id: string): Observable<User> {
    const headers = this.authAdminService.getAdminHeaders();
    return this.http.get<User>(`${this.baseUrl}/${id}`, { headers })
      .pipe(
        catchError(this.handleError)
      );
  }
    getAllUsers(): Observable<User[]> {
    const headers = this.authAdminService.getAdminHeaders();
    return this.http.get<User[]>(`${this.baseUrl}`, { headers })
      .pipe(
        catchError(this.handleError)
      );
  }

  updateUser(userId: string, updateData: UpdateUserDto): Observable<any> {
    const headers = this.authAdminService.getAdminHeaders();
    return this.http.put(`${this.baseUrl}/${userId}`, updateData, { headers })
      .pipe(
        catchError(this.handleError)
      );
  }

  deleteUser(userId: string): Observable<any> {
    const headers = this.authAdminService.getAdminHeaders();
    return this.http.delete(`${this.baseUrl}/${userId}`, { headers })
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(error: any) {
    console.error('An error occurred:', error);
    return throwError(() => error);
  }
}