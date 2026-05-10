import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { ResumenService } from '../../core/services/resumen.service';
import { ResumenMensual } from '../../core/models/resumen.model';
import { MESES } from '../../core/models/quincena.model';
import { CurrencyPipe, DatePipe,NgClass } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NavbarComponent, FormsModule, RouterLink, CurrencyPipe, NgClass, DatePipe],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  private resumenService = inject(ResumenService);

  meses = MESES;
  anioActual = new Date().getFullYear();
  mesActual  = new Date().getMonth() + 1;

  anioSeleccionado = this.anioActual;
  mesSeleccionado  = this.mesActual;

  resumen = signal<ResumenMensual | null>(null);
  cargando = signal(false);
  error = signal('');

  // Años disponibles para el selector
  anios = [this.anioActual - 1, this.anioActual, this.anioActual + 1];

  ngOnInit() {
    this.cargarResumen();
  }

  cargarResumen() {
    this.cargando.set(true);
    this.error.set('');

    this.resumenService.mensual(this.anioSeleccionado, this.mesSeleccionado).subscribe({
      next: (data) => {
        this.resumen.set(data);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No hay datos para este período.');
        this.resumen.set(null);
        this.cargando.set(false);
      }
    });
  }

  // Calcula el porcentaje del presupuesto usado para la barra de progreso
  porcentajeBarra(porcentaje: number | undefined): number {
    if (!porcentaje) return 0;
    return Math.min(porcentaje, 100); // máximo 100% visual
  }

  colorBarra(porcentaje: number | undefined): string {
    if (!porcentaje) return 'bg-emerald-500';
    if (porcentaje >= 100) return 'bg-red-500';
    if (porcentaje >= 80)  return 'bg-yellow-500';
    return 'bg-emerald-500';
  }

  nombreMesSeleccionado(): string {
    return this.meses.find(m => m.valor === this.mesSeleccionado)?.nombre ?? '';
  }
}
