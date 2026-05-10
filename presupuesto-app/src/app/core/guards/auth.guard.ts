import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Guard funcional (Angular 17+)
// Protege las rutas que requieren autenticación
export const authGuard: CanActivateFn = () => {
  const auth   = inject(AuthService);
  const router = inject(Router);

  if (auth.estaAutenticado()) return true;

  // Si no está autenticado, redirige al login
  router.navigate(['/login']);
  return false;
};
