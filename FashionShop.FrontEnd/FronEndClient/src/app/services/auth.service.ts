import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { BehaviorSubject,Observable,tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { LoginRequest, RegisterRequest, AuthResponse } from '../models/auth.model';
 
import { jwtDecode } from 'jwt-decode';
export interface JwtPayload {
  sub: string;
  name: string;
  emailaddress: string;
  role: string;
  exp: number;
  iat: number;
}
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = environment.apiUrl+"/auth";
  private tokenKey = 'abcd-do-giap-123d-oke-at-random-32-bytes-long';
  private isLoggedIn$ = new BehaviorSubject<boolean>(this.hasToken());
  
  get isLoggedIn(): boolean {
    return this.isLoggedIn$ .getValue();
  }
  constructor(private http:HttpClient) { }
  getUserInfo(): JwtPayload|null{
    if (this.hasToken()) {
      const token = this.getToken() as string;
      const decodedToken: JwtPayload = jwtDecode(token);
      console.log('Decoded token:', decodedToken);
      return decodedToken;
    }
    return null;
  }
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
  getUserId():string|null{
    const token = this.getToken();
    if (token) {
      const decodedToken: JwtPayload = jwtDecode(token);
      return decodedToken.sub; // Assuming 'sub' is the user ID
    }
    return null;
  }
  private handleAuthentication(response: AuthResponse): void {
    if (response.token) {
      localStorage.setItem(this.tokenKey, response.token);
      this.isLoggedIn$.next(true);
      console.log('User authenticated successfully:', response);
    } else {
      console.error('Authentication failed: No token received');
    }
  }
  
}
