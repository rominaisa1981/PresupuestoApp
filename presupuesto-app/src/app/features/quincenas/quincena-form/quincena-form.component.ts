import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { QuincenaService } from '../../../core/services/quincena.service';
import { MESES } from '../../../core/models/quincena.model';

@Component({
  selector: 'app-quincena-form',
  standalone: true,
  imports: [NavbarComponent, ReactiveFormsModule, RouterLink],
  templateUrl: './quincena-form.component.html'
})
export class QuincenaFormComponent {
  private fb      = inject(FormBuilder);
  private service = inject(QuincenaService);
  private router  = inject(Router);

  meses  = MESES;
  anios  = [new Date().getFullYear() - 1, new Date().getFullYear(), new Date().getFullYear() + 1];

  form = this.fb.group({
    fechaPago:     ['', Validators.required],
    numero:        [2, [Validators.required, Validators.min(1), Validators.max(2)]],
    mes:           [new Date().getMonth() + 1, Validators.required],
    anio:          [new Date().getFullYear(),  Validators.required],
    observaciones: ['']
  });

  error    = '';
  cargando = false;

  enviar() {
    if (this.form.invalid) return;
    this.cargando = true;
    this.error = '';

    const val = this.form.value;
    this.service.crear({
      fechaPago:     val.fechaPago!,
      numero:        +val.numero!,
      mes:           +val.mes!,
      anio:          +val.anio!,
      observaciones: val.observaciones || undefined
    }).subscribe({
      next:  (q)   => this.router.navigate(['/quincenas', q.id]),
      error: (err) => {
        this.error = err.error?.error ?? 'Error al crear quincena';
        this.cargando = false;
      }
    });
  }
}
