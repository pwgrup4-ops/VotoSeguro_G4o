
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

import { environment } from '../../../environments/environment';
import { AuthResponse, LoginDto, RegisterDto, User } from '../../shared/models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/Auth`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    const token = this.getToken();
    if (token) {
      const user = this.getUserFromToken(token);
      this.currentUserSubject.next(user);
    }
  }

  get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  register(data: RegisterDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, data)
      .pipe(tap(res => this.handleAuthResponse(res)));
  }

  login(data: LoginDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, data)
      .pipe(tap(res => this.handleAuthResponse(res)));
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    try {
      const decoded: any = jwtDecode(token);
      const expMs = decoded.exp * 1000;
      return Date.now() < expMs;
    } catch {
      return false;
    }
  }

  getUserRole(): 'admin' | 'votante' | null {
    const user = this.currentUserValue;
    return user?.role ?? null;
  }

  private handleAuthResponse(response: AuthResponse): void {
    localStorage.setItem('token', response.token);
    const user = this.getUserFromToken(response.token);
    this.currentUserSubject.next(user);
  }

  private getUserFromToken(token: string): User | null {
    try {
      const decoded: any = jwtDecode(token);

      return {
        id: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        fullname: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
        role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      } as User;
    } catch {
      return null;
    }
  }
}

