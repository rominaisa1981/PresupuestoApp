export type TipoMovimiento = 'Ingreso' | 'Descuento' | 'Pago' | 'Gasto';

export interface Categoria {
  id: number;
  nombre: string;
  tipo: TipoMovimiento;
  color?: string;
  codigoRol?: string;
  presupuestoMensual?: number;
  activa: boolean;
}

export interface CrearCategoriaDto {
  nombre: string;
  tipo: TipoMovimiento;
  color?: string;
  codigoRol?: string;
  presupuestoMensual?: number;
}
