

import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-already-voted',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './AlreadyVotedComponent.html',
  styleUrls: ['./AlreadyVotedComponent.css']
})
export class AlreadyVotedComponent {
  candidateName: string | null = null;
  timestamp: string | null = null;

  constructor(
    private router: Router,
    private authService: AuthService
  ) {
    const nav = this.router.getCurrentNavigation();
    const state = nav?.extras.state as { candidateName?: string; timestamp?: string } | undefined;
    this.candidateName = state?.candidateName ?? null;
    this.timestamp = state?.timestamp ?? null;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  goToResults(): void {
    this.router.navigate(['/voter/dashboard']);   // ruta que añadiste en app.routes.ts
  }
}
