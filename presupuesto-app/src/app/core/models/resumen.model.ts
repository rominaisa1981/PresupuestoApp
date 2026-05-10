import { QuincenaDetalle } from './quincena.model';

export interface ResumenMensual {
  mes: number;
  anio: number;
  nombreMes: string;
  totalIngresos: number;
  totalDescuentos: number;
  netoRecibir: number;
  totalPagos: number;
  totalGastos: number;
  saldoFinal: number;
  quincenas: QuincenaDetalle[];
  resumenPorCategoria: ResumenCategoria[];
}

export interface ResumenCategoria {
  categoriaId?: number;
  categoriaNombre: string;
  color?: string;
  total: number;
  presupuestoMensual?: number;
  porcentajeUso?: number;
}
