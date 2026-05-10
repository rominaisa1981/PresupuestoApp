import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Movimiento, CrearMovimientoDto, ActualizarMovimientoDto } from '../models/movimiento.model';

@Injectable({ providedIn: 'root' })
export class MovimientoService {
  private url = `${environment.apiUrl}/movimientos`;
  constructor(private http: HttpClient) {}

  obtener(id: number) {
    return this.http.get<Movimiento>(`${this.url}/${id}`);
  }

  crear(dto: CrearMovimientoDto) {
    return this.http.post<Movimiento>(this.url, dto);
  }

  actualizar(id: number, dto: ActualizarMovimientoDto) {
    return this.http.put<Movimiento>(`${this.url}/${id}`, dto);
  }

  eliminar(id: number) {
    return this.http.delete(`${this.url}/${id}`);
  }
}
