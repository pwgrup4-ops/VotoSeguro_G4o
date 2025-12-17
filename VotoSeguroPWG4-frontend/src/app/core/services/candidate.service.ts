
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Candidate } from '../../shared/models/candidate';
import { CandidateDTo } from '../../shared/models/CandidateDTo';

@Injectable({ providedIn: 'root' })
export class CandidateService {
  private apiUrl = `${environment.apiUrl}/Candidates`;

  constructor(private http: HttpClient) {}

  // Lista todos los candidatos
  getAll(): Observable<Candidate[]> {
    return this.http.get<Candidate[]>(this.apiUrl);
  }

  // Obtiene un candidato por id
  getById(id: string): Observable<Candidate> {
    return this.http.get<Candidate>(`${this.apiUrl}/${id}`);
  }

  // Crea un nuevo candidato (admin)
  create(dto: CandidateDTo): Observable<Candidate> {
    return this.http.post<Candidate>(this.apiUrl, dto);
  }

  // Actualiza un candidato existente (admin)
  update(id: string, dto: CandidateDTo): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  // Elimina un candidato (admin) — deberás validar en el componente que votesCount === 0
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
