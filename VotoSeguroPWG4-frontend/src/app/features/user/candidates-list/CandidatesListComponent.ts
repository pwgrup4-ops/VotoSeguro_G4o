
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { CandidateService } from '../../../core/services/candidate.service';
import { Candidate } from '../../../shared/models/candidate';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-candidates-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './CandidatesListComponent.html',
  styleUrls: ['./CandidatesListComponent.css']
})
export class CandidatesListComponent implements OnInit {
  candidates: Candidate[] = [];
  loading = true;
  error: string | null = null;

  constructor(
    private candidateService: CandidateService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCandidates();
  }

  loadCandidates(): void {
    this.loading = true;
    this.error = null;

    this.candidateService.getAll().subscribe({
      next: (data) => {
        this.candidates = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se pudieron cargar los candidatos';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  goToVote(id: string): void {
    this.router.navigate(['/voter/voting', id]);
  }

  logout(): void {
    this.authService.logout(); // limpia token y navega a /login
  }
}





