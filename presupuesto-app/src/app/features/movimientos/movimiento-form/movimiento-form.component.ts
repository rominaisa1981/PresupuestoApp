import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { MovimientoService } from '../../../core/services/movimiento.service';
import { CategoriaService } from '../../../core/services/categoria.service';
import { Categoria, TipoMovimiento } from '../../../core/models/categoria.model';
import { Movimiento } from '../../../core/models/movimiento.model';

@Component({
  selector: 'app-movimiento-form',
  standalone: true,
  imports: [NavbarComponent, ReactiveFormsModule, RouterLink],
  templateUrl: './movimiento-form.component.html'
})
export class MovimientoFormComponent implements OnInit {
  private fb       = inject(FormBuilder);
  private route    = inject(ActivatedRoute);
  private router   = inject(Router);
  private movServ  = inject(MovimientoService);
  private catServ  = inject(CategoriaService);

  quincenaId = +this.route.snapshot.paramMap.get('id')!;

  tipos: TipoMovimiento[] = ['Ingreso', 'Descuento', 'Pago', 'Gasto'];
  categorias = signal<Categoria[]>([]);
  error    = '';
  cargando = false;

  form = this.fb.group({
    descripcion:        ['', [Validators.required, Validators.minLength(2)]],
    monto:              [null as number | null, [Validators.required, Validators.min(0.01)]],
    tipo:               ['Ingreso' as TipoMovimiento, Validators.required],
    categoriaId:        [null as number | null],
    movimientoPadreId:  [null as number | null],
    notas:              ['']
  });

  ngOnInit() {
    // Cargar categorías para el tipo inicial
    this.cargarCategorias('Ingreso');

    // Cuando cambia el tipo, recarga las categorías filtradas y limpia la selección
    this.form.get('tipo')!.valueChanges.subscribe(tipo => {
      if (tipo) {
        this.cargarCategorias(tipo);
        this.form.patchValue({ categoriaId: null });
      }
    });
  }

  cargarCategorias(tipo: TipoMovimiento) {
    this.catServ.listar(tipo).subscribe({
      next: (cats) => this.categorias.set(cats.filter(c => c.activa))
    });
  }

  enviar() {
    if (this.form.invalid) return;
    this.cargando = true;
    this.error = '';

    const val = this.form.value;

    this.movServ.crear({
      descripcion:       val.descripcion!,
      monto:             +val.monto!,
      tipo:              val.tipo!,
      quincenaId:        this.quincenaId,
      categoriaId:       val.categoriaId ?? undefined,
      movimientoPadreId: val.movimientoPadreId ?? undefined,
      notas:             val.notas || undefined
    }).subscribe({
      next:  () => this.router.navigate(['/quincenas', this.quincenaId]),
      error: (err) => {
        this.error = err.error?.error ?? 'Error al guardar el movimiento';
        this.cargando = false;
      }
    });
  }
}
