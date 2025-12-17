
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CandidateResult {
  candidateId: string;
  candidateName: string;
  party: string;
  voteCount: number;
  votePercentage: number;
}

export interface VoteStatisticsResponse {
  totalRegisteredVoters: number;
  totalVotesCast: number;
  participationRate: number;
  candidateResults: CandidateResult[];
}

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private apiUrl = `${environment.apiUrl}/Reports`; // apiUrl = https://localhost:5132/api

  constructor(private http: HttpClient) {}

  getVoteStatistics(): Observable<VoteStatisticsResponse> {
    return this.http.get<VoteStatisticsResponse>(`${this.apiUrl}/statistics`);
  }
}
