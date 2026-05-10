import { Movimiento } from './movimiento.model';

export interface Quincena {
  id: number;
  fechaPago: string;
  numero: number;
  mes: number;
  anio: number;
  observaciones?: string;
}

export interface QuincenaDetalle extends Quincena {
  totalIngresos: number;
  totalDescuentos: number;
  netoRecibir: number;
  totalPagos: number;
  totalGastos: number;
  saldo: number;
  ingresos: Movimiento[];
  descuentos: Movimiento[];
  pagos: Movimiento[];
  gastos: Movimiento[];
}

export interface CrearQuincenaDto {
  fechaPago: string;
  numero: number;
  mes: number;
  anio: number;
  observaciones?: string;
}

export const MESES: { valor: number; nombre: string }[] = [
  { valor: 1, nombre: 'Enero' }, { valor: 2, nombre: 'Febrero' },
  { valor: 3, nombre: 'Marzo' }, { valor: 4, nombre: 'Abril' },
  { valor: 5, nombre: 'Mayo' }, { valor: 6, nombre: 'Junio' },
  { valor: 7, nombre: 'Julio' }, { valor: 8, nombre: 'Agosto' },
  { valor: 9, nombre: 'Septiembre' }, { valor: 10, nombre: 'Octubre' },
  { valor: 11, nombre: 'Noviembre' }, { valor: 12, nombre: 'Diciembre' },
];
