import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, BehaviorSubject, tap, catchError, throwError } from 'rxjs';
import { 
  LoginRequest, 
  RegisterRequest, 
  AuthResponse, 
  User, 
  UpdateProfileRequest, 
  ChangePasswordRequest 
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = 'http://localhost:5118/api';
  private readonly TOKEN_KEY = 'shopforhome_token';
  private readonly USER_KEY = 'shopforhome_user';

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  private tokenSubject = new BehaviorSubject<string | null>(null);

  // Signals for reactive state management
  private _currentUser = signal<User | null>(null);
  private _isAuthenticated = signal<boolean>(false);
  private _isLoading = signal<boolean>(false);

  // Public computed signals
  public currentUser = this._currentUser.asReadonly();
  public isAuthenticated = this._isAuthenticated.asReadonly();
  public isLoading = this._isLoading.asReadonly();
  public isAdmin = computed(() => this._currentUser()?.role === 'Admin');

  // Observable streams
  public currentUser$ = this.currentUserSubject.asObservable();
  public token$ = this.tokenSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.initializeAuth();
  }

  private initializeAuth(): void {
    const token = this.getStoredToken();
    const user = this.getStoredUser();

    if (token && user) {
      this.setAuthState(user, token);
    }
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    this._isLoading.set(true);
    
    return this.http.post<AuthResponse>(`${this.API_URL}/Auth/login`, credentials)
      .pipe(
        tap(response => {
          this.handleAuthSuccess(response);
        }),
        catchError(error => {
          this._isLoading.set(false);
          return throwError(() => error);
        })
      );
  }

  register(userData: RegisterRequest): Observable<AuthResponse> {
    this._isLoading.set(true);
    
    return this.http.post<AuthResponse>(`${this.API_URL}/Auth/register`, userData)
      .pipe(
        tap(response => {
          this.handleAuthSuccess(response);
        }),
        catchError(error => {
          this._isLoading.set(false);
          return throwError(() => error);
        })
      );
  }

  logout(): void {
    this.clearAuthState();
    this.router.navigate(['/login']);
  }

  getProfile(): Observable<User> {
    return this.http.get<User>(`${this.API_URL}/Auth/profile`);
  }

  updateProfile(profileData: UpdateProfileRequest): Observable<any> {
    return this.http.put(`${this.API_URL}/Auth/profile`, profileData)
      .pipe(
        tap(() => {
          // Refresh user profile after update
          this.getProfile().subscribe(user => {
            this.updateUserState(user);
          });
        })
      );
  }

  changePassword(passwordData: ChangePasswordRequest): Observable<any> {
    return this.http.post(`${this.API_URL}/Auth/change-password`, passwordData);
  }

  checkEmailExists(email: string): Observable<{ exists: boolean }> {
    return this.http.get<{ exists: boolean }>(`${this.API_URL}/Auth/check-email?email=${email}`);
  }

  refreshToken(): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.API_URL}/Auth/refresh-token`, {})
      .pipe(
        tap(response => {
          this.handleAuthSuccess(response);
        })
      );
  }

  // Token management
  getToken(): string | null {
    return this.tokenSubject.value || this.getStoredToken();
  }

  private getStoredToken(): string | null {
    if (typeof window !== 'undefined') {
      return localStorage.getItem(this.TOKEN_KEY);
    }
    return null;
  }

  private getStoredUser(): User | null {
    if (typeof window !== 'undefined') {
      const userJson = localStorage.getItem(this.USER_KEY);
      return userJson ? JSON.parse(userJson) : null;
    }
    return null;
  }

  private handleAuthSuccess(response: AuthResponse): void {
    this.setAuthState(response.user, response.token);
    this.storeAuthData(response.user, response.token);
    this._isLoading.set(false);
  }

  private setAuthState(user: User, token: string): void {
    this._currentUser.set(user);
    this._isAuthenticated.set(true);
    this.currentUserSubject.next(user);
    this.tokenSubject.next(token);
  }

  private updateUserState(user: User): void {
    this._currentUser.set(user);
    this.currentUserSubject.next(user);
    if (typeof window !== 'undefined') {
      localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    }
  }

  private storeAuthData(user: User, token: string): void {
    if (typeof window !== 'undefined') {
      localStorage.setItem(this.TOKEN_KEY, token);
      localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    }
  }

  private clearAuthState(): void {
    this._currentUser.set(null);
    this._isAuthenticated.set(false);
    this.currentUserSubject.next(null);
    this.tokenSubject.next(null);
    
    if (typeof window !== 'undefined') {
      localStorage.removeItem(this.TOKEN_KEY);
      localStorage.removeItem(this.USER_KEY);
    }
  }

  // Helper methods for route guards
  canActivate(): boolean {
    return this._isAuthenticated();
  }

  canActivateAdmin(): boolean {
    return this._isAuthenticated() && this.isAdmin();
  }

  // Check if token is expired
  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp * 1000;
      return Date.now() > expiry;
    } catch {
      return true;
    }
  }
}