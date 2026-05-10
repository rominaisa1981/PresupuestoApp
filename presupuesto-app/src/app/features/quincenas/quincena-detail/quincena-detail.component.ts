import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { QuincenaService } from '../../../core/services/quincena.service';
import { MovimientoService } from '../../../core/services/movimiento.service';
import { QuincenaDetalle } from '../../../core/models/quincena.model';
import { MESES } from '../../../core/models/quincena.model';
import { CurrencyPipe, DatePipe, NgClass } from '@angular/common';

@Component({
  selector: 'app-quincena-detail',
  standalone: true,
  imports: [NavbarComponent, RouterLink, CurrencyPipe, DatePipe, NgClass, ReactiveFormsModule],
  templateUrl: './quincena-detail.component.html'
})
export class QuincenaDetailComponent implements OnInit {
  private route   = inject(ActivatedRoute);
  private router  = inject(Router);
  private fb      = inject(FormBuilder);
  private service = inject(QuincenaService);
  private movServ = inject(MovimientoService);

  quincena   = signal<QuincenaDetalle | null>(null);
  cargando   = signal(true);
  expandidos = new Set<number>();

  // ── Modal copiar ──────────────────────────────────
  mostrarModalCopiar = signal(false);
  copiando = false;
  errorCopia = '';
  meses = MESES;
  anios = [new Date().getFullYear() - 1, new Date().getFullYear(), new Date().getFullYear() + 1];

  formCopiar = this.fb.group({
    mes:       [new Date().getMonth() + 1, [Validators.required, Validators.min(1), Validators.max(12)]],
    anio:      [new Date().getFullYear(),  Validators.required],
    fechaPago: ['', Validators.required]
  });

  // ── Modal editar movimiento ───────────────────────
  mostrarModalEditar = signal(false);
  editandoId = signal<number | null>(null);
  errorEdicion = '';
  guardandoEdicion = false;

  formEditar = this.fb.group({
    descripcion: ['', [Validators.required, Validators.minLength(2)]],
    monto:       [null as number | null, [Validators.required, Validators.min(0.01)]],
    notas:       ['']
  });

  get quincenaId(): number {
    return +this.route.snapshot.paramMap.get('id')!;
  }

  ngOnInit() { this.cargar(); }

  cargar() {
    this.cargando.set(true);
    this.service.obtenerDetalle(this.quincenaId).subscribe({
      next:  (data) => { this.quincena.set(data); this.cargando.set(false); },
      error: ()     => { this.cargando.set(false); this.router.navigate(['/quincenas']); }
    });
  }

  toggleExpandido(id: number) {
    this.expandidos.has(id) ? this.expandidos.delete(id) : this.expandidos.add(id);
  }

  // ── Editar ────────────────────────────────────────
  abrirEditar(mov: { id: number; descripcion: string; monto: number; notas?: string }) {
    this.editandoId.set(mov.id);
    this.errorEdicion = '';
    this.formEditar.setValue({
      descripcion: mov.descripcion,
      monto:       mov.monto,
      notas:       mov.notas ?? ''
    });
    this.mostrarModalEditar.set(true);
  }

  cerrarEditar() {
    this.mostrarModalEditar.set(false);
    this.editandoId.set(null);
  }

  guardarEdicion() {
    if (this.formEditar.invalid || !this.editandoId()) return;
    this.guardandoEdicion = true;
    this.errorEdicion = '';

    const val = this.formEditar.value;
    this.movServ.actualizar(this.editandoId()!, {
      descripcion: val.descripcion!,
      monto:       +val.monto!,
      notas:       val.notas || undefined
    }).subscribe({
      next: () => { this.cerrarEditar(); this.cargar(); },
      error: (err) => {
        this.errorEdicion = err.error?.error ?? 'Error al guardar';
        this.guardandoEdicion = false;
      }
    });
  }

  eliminarMovimiento(id: number) {
    if (!confirm('¿Eliminar este movimiento?')) return;
    this.movServ.eliminar(id).subscribe(() => this.cargar());
  }

  eliminarQuincena() {
    if (!confirm('¿Eliminar esta quincena y todos sus movimientos?')) return;
    this.service.eliminar(this.quincenaId).subscribe(() =>
      this.router.navigate(['/quincenas'])
    );
  }

  // ── Copiar ────────────────────────────────────────
  abrirCopiar() {
    this.errorCopia = '';
    this.mostrarModalCopiar.set(true);
  }

  cerrarCopiar() {
    this.mostrarModalCopiar.set(false);
    this.errorCopia = '';
  }

  confirmarCopia() {
    if (this.formCopiar.invalid) return;
    this.copiando = true;
    this.errorCopia = '';

    const val = this.formCopiar.value;
    this.service.copiar(
      this.quincenaId,
      +val.mes!,
      +val.anio!,
      val.fechaPago || undefined
    ).subscribe({
      next:  (nueva) => this.router.navigate(['/quincenas', nueva.id]),
      error: (err)   => {
        this.errorCopia = err.error?.error ?? 'Error al copiar';
        this.copiando = false;
      }
    });
  }

  agregarMovimiento() {
    this.router.navigate(['/quincenas', this.quincenaId, 'movimiento']);
  }
}
