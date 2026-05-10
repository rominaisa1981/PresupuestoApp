import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },

  // Rutas públicas
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component')
      .then(m => m.LoginComponent)
  },
  {
    path: 'registro',
    loadComponent: () => import('./features/auth/register/register.component')
      .then(m => m.RegisterComponent)
  },

  // Rutas protegidas (requieren login)
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component')
      .then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'quincenas',
    loadComponent: () => import('./features/quincenas/quincena-list/quincena-list.component')
      .then(m => m.QuincenaListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'quincenas/nueva',
    loadComponent: () => import('./features/quincenas/quincena-form/quincena-form.component')
      .then(m => m.QuincenaFormComponent),
    canActivate: [authGuard]
  },
  {
    path: 'quincenas/:id',
    loadComponent: () => import('./features/quincenas/quincena-detail/quincena-detail.component')
      .then(m => m.QuincenaDetailComponent),
    canActivate: [authGuard]
  },
  {
    path: 'quincenas/:id/movimiento',
    loadComponent: () => import('./features/movimientos/movimiento-form/movimiento-form.component')
      .then(m => m.MovimientoFormComponent),
    canActivate: [authGuard]
  },

  // Fallback
  { path: '**', redirectTo: 'dashboard' }
];
