
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CandidateService } from '../../../core/services/candidate.service';
import { Candidate } from '../../../shared/models/candidate';

@Component({
  selector: 'app-voter-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './voter-dashboard.html',
  styleUrls: ['./voter-dashboard.css']
})
export class VoterDashboardComponent implements OnInit {
  loading = true;
  error: string | null = null;

  totalVotes = 0;
  stats: {
    name: string;
    party: string;
    votes: number;
    percentage: number;
  }[] = [];

  constructor(
    private candidateService: CandidateService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadStats();
  }

  private loadStats(): void {
    this.loading = true;
    this.error = null;

    this.candidateService.getAll().subscribe({
      next: (data: Candidate[]) => {
        this.totalVotes = data.reduce(
          (acc, c) => acc + (c.voteCount ?? 0),
          0
        );

        this.stats = data.map(c => {
          const votes = c.voteCount ?? 0;
          const percentage = this.totalVotes
            ? (votes * 100) / this.totalVotes
            : 0;

          return {
            name: c.name,
            party: c.party,
            votes,
            percentage
          };
        });

        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se pudieron cargar los resultados';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/voter/candidates']);
  }
}



