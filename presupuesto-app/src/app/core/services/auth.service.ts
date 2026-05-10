import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginDto, RegisterDto, UsuarioDto } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'presupuesto_token';
  private readonly USER_KEY  = 'presupuesto_user';

  // Signal reactivo — cualquier componente que lo lea se actualiza automáticamente
  usuario = signal<UsuarioDto | null>(this.cargarUsuario());

  constructor(private http: HttpClient, private router: Router) {}

  registro(dto: RegisterDto) {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/registro`, dto)
      .pipe(tap(resp => this.guardarSesion(resp)));
  }

  login(dto: LoginDto) {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, dto)
      .pipe(tap(resp => this.guardarSesion(resp)));
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.usuario.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  estaAutenticado(): boolean {
    return !!this.getToken();
  }

  private guardarSesion(resp: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, resp.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(resp.usuario));
    this.usuario.set(resp.usuario);
  }

  private cargarUsuario(): UsuarioDto | null {
    const json = localStorage.getItem(this.USER_KEY);
    return json ? JSON.parse(json) : null;
  }
}
