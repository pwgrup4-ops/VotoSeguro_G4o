
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CandidateService } from '../../../core/services/candidate.service';
import { Candidate } from '../../../shared/models/candidate';
import { VoteService } from '../../../core/services/VoteService';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-voting',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './VotingComponent.html',
  styleUrls: ['./VotingComponent.css']
})
export class VotingComponent implements OnInit {
  candidate: Candidate | null = null;
  loading = true;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private candidateService: CandidateService,
    private voteService: VoteService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error = 'Candidato inválido';
      this.loading = false;
      return;
    }

    this.candidateService.getById(id).subscribe({
      next: (c) => {
        this.candidate = c;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se pudo cargar el candidato';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  confirmVote(): void {
    if (!this.candidate) return;

    this.loading = true;
    this.error = null;

    this.voteService.voteFor(this.candidate.id).subscribe({
      next: () => {
        this.loading = false;
        this.cdr.detectChanges();
        this.router.navigate(['/voter/vote-confirmation'], {
          state: { candidate: this.candidate }
        });
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error || 'No se pudo registrar el voto';
        this.cdr.detectChanges();
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/voter/candidates']);
  }

  logout(): void {
    this.authService.logout(); // internamente ya hace this.router.navigate(['/login'])
  }
}

