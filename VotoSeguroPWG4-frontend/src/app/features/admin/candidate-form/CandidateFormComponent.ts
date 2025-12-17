
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { CandidateService } from '../../../core/services/candidate.service';
import { Candidate } from '../../../shared/models/candidate';
import { CandidateDTo } from '../../../shared/models/CandidateDTo';

@Component({
  selector: 'app-candidate-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './CandidateFormComponent.html',
  styleUrls: ['./CandidateFormComponent.css'],
})
export class CandidateFormComponent implements OnInit {
  candidateForm!: FormGroup;
  isEditMode = false;
  candidateId: string | null = null;

  loadingSubmit = false;
  submitError: string | null = null;
  successMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private candidateService: CandidateService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.candidateForm = this.fb.group({
      name: ['', Validators.required],
      party: ['', Validators.required],
      photoUrl: ['', Validators.required],
      logoUrl: ['', Validators.required],
      proposalsText: ['', Validators.required], // una propuesta por línea
    });

    this.candidateId = this.route.snapshot.paramMap.get('id');
    if (this.candidateId) {
      this.isEditMode = true;
      this.loadCandidateData(this.candidateId);
    }
  }

  private loadCandidateData(id: string): void {
    this.candidateService.getById(id).subscribe({
      next: (candidate: Candidate) => {
        this.candidateForm.patchValue({
          name: candidate.name,
          party: candidate.party,
          photoUrl: candidate.photoUrl || '',
          logoUrl: candidate.logoUrl || '',
          proposalsText: candidate.proposals?.join('\n') || '',
        });
      },
      error: () => {
        this.submitError = 'No se pudo cargar el candidato.';
      }
    });
  }

  onSubmit(): void {
    if (this.candidateForm.invalid) {
      this.candidateForm.markAllAsTouched();
      return;
    }

    this.submitError = null;
    this.loadingSubmit = true;

    const formData = this.candidateForm.value;

    const dto: CandidateDTo = {
      name: formData.name,
      party: formData.party,
      photoUrl: formData.photoUrl,
      logoUrl: formData.logoUrl,
      proposals: formData.proposalsText
        .split('\n')
        .map((x: string) => x.trim())
        .filter((x: string) => x !== '')
    };

    const request$: Observable<any> =
      this.isEditMode && this.candidateId
        ? this.candidateService.update(this.candidateId, dto)
        : this.candidateService.create(dto);

    request$.subscribe({
      next: () => {
        this.loadingSubmit = false;
        this.successMessage = 'Guardado con éxito';
        setTimeout(() => this.router.navigate(['/admin/candidates']), 1500);
      },
      error: () => {
        this.loadingSubmit = false;
        this.submitError = 'Error al guardar.';
      }
    });
  }
}
