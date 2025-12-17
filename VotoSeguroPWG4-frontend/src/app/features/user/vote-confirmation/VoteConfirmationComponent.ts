

import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-vote-confirmation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './VoteConfirmationComponent.html',
  styleUrls: ['./VoteConfirmationComponent.css']
})
export class VoteConfirmationComponent {
  candidateName: string | null = null;
  timestamp: string | null = null;

  constructor(private router: Router) {
    const nav = this.router.getCurrentNavigation();
    const state = nav?.extras.state as { candidateName?: string; timestamp?: string } | undefined;
    this.candidateName = state?.candidateName ?? null;
    this.timestamp = state?.timestamp ?? null;
  }

  volver(): void {
    this.router.navigate(['/voter/candidates']);
  }
}
