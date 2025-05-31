import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';
import { BehaviorSubject, Observable, tap } from 'rxjs';
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
  private apiUrl = environment.apiUrl + "/auth";
  private tokenKey = 'abcd-do-giap-123d-oke-at-random-32-bytes-long';
  private isLoggedIn$ = new BehaviorSubject<boolean>(this.hasToken());
  
  // Observable cho việc theo dõi auth state changes
  public authStateChanged$ = this.isLoggedIn$.asObservable();
  
  get isLoggedIn(): boolean {
    return this.isLoggedIn$.getValue();
  }

  constructor(private http: HttpClient) {
    // Kiểm tra token khi service được khởi tạo
    this.checkTokenExpiration();
  }

  getUserInfo(): JwtPayload | null {
    if (this.hasToken()) {
      try {
        const token = this.getToken() as string;
        const decodedToken: JwtPayload = jwtDecode(token);
        
        // Kiểm tra xem token có hết hạn không
        if (this.isTokenExpired(decodedToken)) {
          this.logout();
          return null;
        }
        
        console.log('Decoded token:', decodedToken);
        return decodedToken;
      } catch (error) {
        console.error('Error decoding token:', error);
        this.logout();
        return null;
      }
    }
    return null;
  }

  // Kiểm tra token có tồn tại không
  hasToken(): boolean {
    const token = localStorage.getItem(this.tokenKey);
    if (!token) {
      return false;
    }
    
    try {
      const decodedToken: JwtPayload = jwtDecode(token);
      return !this.isTokenExpired(decodedToken);
    } catch (error) {
      console.error('Invalid token:', error);
      localStorage.removeItem(this.tokenKey);
      return false;
    }
  }

  // Kiểm tra token có hết hạn không
  private isTokenExpired(decodedToken: JwtPayload): boolean {
    const currentTime = Math.floor(Date.now() / 1000);
    return decodedToken.exp < currentTime;
  }

  // Kiểm tra và xử lý token hết hạn
  private checkTokenExpiration(): void {
    if (this.hasToken()) {
      const userInfo = this.getUserInfo();
      if (!userInfo) {
        this.logout();
      }
    }
  }

  // Hàm đăng ký
  register(registerRequest: RegisterRequest): Observable<AuthResponse> {
    console.log('Registering user:', registerRequest);
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, registerRequest)
      .pipe(
        tap(response => this.handleAuthentication(response))
      );
  }

  // Hàm login
  login(loginRequest: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, loginRequest)
      .pipe(
        tap(response => this.handleAuthentication(response))
      );
  }
  
  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.isLoggedIn$.next(false);
    console.log('User logged out');
  }

  // Lấy token
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  // Lấy user ID từ token
  getUserId(): string | null {
    const userInfo = this.getUserInfo();
    return userInfo ? userInfo.sub : null;
  }

  // Lấy user name từ token
  getUserName(): string | null {
    const userInfo = this.getUserInfo();
    return userInfo ? userInfo.name : null;
  }

  // Lấy user email từ token
  getUserEmail(): string | null {
    const userInfo = this.getUserInfo();
    return userInfo ? userInfo.emailaddress : null;
  }

  // Lấy user role từ token
  getUserRole(): string | null {
    const userInfo = this.getUserInfo();
    return userInfo ? userInfo.role : null;
  }

  // Kiểm tra user có role cụ thể không
  hasRole(role: string): boolean {
    const userRole = this.getUserRole();
    return userRole === role;
  }

  // Refresh token (nếu backend hỗ trợ)
  refreshToken(): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/refresh`, {})
      .pipe(
        tap(response => this.handleAuthentication(response))
      );
  }

  private handleAuthentication(response: AuthResponse): void {
    if (response.token) {
      localStorage.setItem(this.tokenKey, response.token);
      this.isLoggedIn$.next(true);
      console.log('User authenticated successfully:', response);
    } else {
      console.error('Authentication failed: No token received');
      this.isLoggedIn$.next(false);
    }
  }

  // Method để force check auth state (useful for debugging)
  forceCheckAuthState(): void {
    const hasValidToken = this.hasToken();
    this.isLoggedIn$.next(hasValidToken);
  }
}