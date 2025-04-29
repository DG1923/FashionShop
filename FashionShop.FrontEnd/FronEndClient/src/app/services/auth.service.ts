import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { BehaviorSubject,Observable,tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { LoginRequest, RegisterRequest, AuthResponse } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = environment.apiUrl+"/Auth";
  private tokenKey = 'access_token';
  private isLoggedIn$ = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http:HttpClient) { }

  //check token exits
  hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
  //hàm đăng ký
  register(registerRequest: RegisterRequest): Observable<AuthResponse> {
    console.log('Registering user:', registerRequest);
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, registerRequest)
      .pipe(
        tap(response => this.handleAuthentication(response))
      );
  }
  //hàm login
  login(loginRequest: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, loginRequest)
      .pipe(
        tap(response => this.handleAuthentication(response))
      );
  }
  
  logout():void{
    localStorage.removeItem(this.tokenKey);
    this.isLoggedIn$.next(false);
  }
  //take token
  getToken():string|null{
    return localStorage.getItem(this.tokenKey);
  }

  private handleAuthentication(response: AuthResponse): void {
    localStorage.setItem(this.tokenKey, response.token);
    this.isLoggedIn$.next(true);
  }
}
