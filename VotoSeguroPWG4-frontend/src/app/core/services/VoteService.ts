import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface VoteRequest {
  candidateId: string;   // GUID del candidato
}

@Injectable({ providedIn: 'root' })
export class VoteService {
  private apiUrl = `${environment.apiUrl}/Votes`;

  constructor(private http: HttpClient) {}

  voteFor(candidateId: string): Observable<any> {
    const body: VoteRequest = { candidateId };
    return this.http.post<any>(`${this.apiUrl}/cast`, body);
  }


  getStatus(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/status`);
  }


}



