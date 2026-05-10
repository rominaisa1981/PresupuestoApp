export interface RegisterDto {
  email: string;
  nombre: string;
  password: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expira: string;
  usuario: UsuarioDto;
}

export interface UsuarioDto {
  id: number;
  email: string;
  nombre: string;
}
