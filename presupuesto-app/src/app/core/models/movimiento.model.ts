import { TipoMovimiento } from './categoria.model';

export interface Movimiento {
  id: number;
  descripcion: string;
  monto: number;
  tipo: TipoMovimiento;
  fecha: string;
  notas?: string;
  quincenaId: number;
  categoriaId?: number;
  categoriaNombre?: string;
  movimientoPadreId?: number;
  subMovimientos: Movimiento[];
}

export interface CrearMovimientoDto {
  descripcion: string;
  monto: number;
  tipo: TipoMovimiento;
  quincenaId: number;
  categoriaId?: number;
  movimientoPadreId?: number;
  fecha?: string;
  notas?: string;
}

export interface ActualizarMovimientoDto {
  descripcion: string;
  monto: number;
  categoriaId?: number;
  notas?: string;
}
