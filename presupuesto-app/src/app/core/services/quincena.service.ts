import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Quincena, QuincenaDetalle, CrearQuincenaDto } from '../models/quincena.model';

@Injectable({ providedIn: 'root' })
export class QuincenaService {
  private url = `${environment.apiUrl}/quincenas`;
  constructor(private http: HttpClient) {}

  listar(anio?: number, mes?: number) {
    const params = new URLSearchParams();
    if (anio) params.set('anio', anio.toString());
    if (mes)  params.set('mes',  mes.toString());
    const query = params.toString() ? `?${params}` : '';
    return this.http.get<Quincena[]>(`${this.url}${query}`);
  }

  obtenerDetalle(id: number) {
    return this.http.get<QuincenaDetalle>(`${this.url}/${id}`);
  }

  crear(dto: CrearQuincenaDto) {
    return this.http.post<Quincena>(this.url, dto);
  }

  eliminar(id: number) {
    return this.http.delete(`${this.url}/${id}`);
  }

  copiar(id: number, mes: number, anio: number, fechaPago?: string) {
    return this.http.post<Quincena>(`${this.url}/${id}/copiar`, { mes, anio, fechaPago });
  }
}
