
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { CandidateService } from '../../../core/services/candidate.service';
import { Candidate } from '../../../shared/models/candidate';

@Component({
  selector: 'app-admin-candidates-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './CandidatesListComponent.html',
  styleUrls: ['./CandidatesListComponent.css']
})
export class CandidatesListComponent implements OnInit {
  candidates: Candidate[] = [];
  loading = true;
  error: string | null = null;
  deletingId: string | null = null;

  constructor(
    private candidateService: CandidateService,
    private router: Router,
    private cdr: ChangeDetectorRef
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
        this.error = 'No se pudieron cargar los candidatos.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/admin']);   // panel de administración
  }

  goToCreate(): void {
    this.router.navigate(['/admin/candidates/new']);
  }

  goToEdit(candidate: Candidate): void {
    this.router.navigate(['/admin/candidates', candidate.id, 'edit']);
  }

  canDelete(candidate: Candidate): boolean {
    return !candidate.voteCount || candidate.voteCount === 0;
  }

  deleteCandidate(candidate: Candidate): void {
    if (!this.canDelete(candidate)) {
      this.error = 'No se puede eliminar un candidato que ya tiene votos.';
      return;
    }

    if (!confirm(`¿Eliminar al candidato "${candidate.name}"?`)) {
      return;
    }

    this.deletingId = candidate.id;

    this.candidateService.delete(candidate.id).subscribe({
      next: () => {
        this.deletingId = null;
        this.candidates = this.candidates.filter(c => c.id !== candidate.id);
        this.cdr.detectChanges();
      },
      error: () => {
        this.deletingId = null;
        this.error = 'Error al eliminar el candidato.';
        this.cdr.detectChanges();
      }
    });
  }
}

