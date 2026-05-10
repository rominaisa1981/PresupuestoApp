import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Categoria, CrearCategoriaDto, TipoMovimiento } from '../models/categoria.model';

@Injectable({ providedIn: 'root' })
export class CategoriaService {
  private url = `${environment.apiUrl}/categorias`;
  constructor(private http: HttpClient) {}

  listar(tipo?: TipoMovimiento) {
    const params = tipo ? `?tipo=${tipo}` : '';
    return this.http.get<Categoria[]>(`${this.url}${params}`);
  }

  crear(dto: CrearCategoriaDto) {
    return this.http.post<Categoria>(this.url, dto);
  }

  actualizar(id: number, dto: Partial<CrearCategoriaDto>) {
    return this.http.put<Categoria>(`${this.url}/${id}`, dto);
  }
}
