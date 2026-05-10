import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ResumenMensual } from '../models/resumen.model';

@Injectable({ providedIn: 'root' })
export class ResumenService {
  private url = `${environment.apiUrl}/resumen`;
  constructor(private http: HttpClient) {}

  mensual(anio: number, mes: number) {
    return this.http.get<ResumenMensual>(`${this.url}/mensual/${anio}/${mes}`);
  }
}
