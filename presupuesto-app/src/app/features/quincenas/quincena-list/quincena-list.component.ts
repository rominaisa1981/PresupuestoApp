import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { QuincenaService } from '../../../core/services/quincena.service';
import { Quincena, MESES } from '../../../core/models/quincena.model';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-quincena-list',
  standalone: true,
  imports: [NavbarComponent, RouterLink, DatePipe],
  templateUrl: './quincena-list.component.html'
})
export class QuincenaListComponent implements OnInit {
  private service = inject(QuincenaService);

  quincenas = signal<Quincena[]>([]);
  cargando  = signal(true);

  ngOnInit() {
    this.service.listar().subscribe({
      next: (data) => { this.quincenas.set(data); this.cargando.set(false); },
      error: ()    => this.cargando.set(false)
    });
  }

  // Agrupa las quincenas por "Año - Mes"
  get quincenasAgrupadas(): { etiqueta: string; items: Quincena[] }[] {
    const grupos = new Map<string, Quincena[]>();

    for (const q of this.quincenas()) {
      const mes    = MESES.find(m => m.valor === q.mes)?.nombre ?? q.mes;
      const clave  = `${mes} ${q.anio}`;
      if (!grupos.has(clave)) grupos.set(clave, []);
      grupos.get(clave)!.push(q);
    }

    return Array.from(grupos.entries())
      .map(([etiqueta, items]) => ({ etiqueta, items }));
  }
}
