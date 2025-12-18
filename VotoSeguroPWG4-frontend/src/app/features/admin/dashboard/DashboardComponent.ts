
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './DashboardComponent.html',
  styleUrls: ['./DashboardComponent.css']
})
export class DashboardComponent {
  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  goToCandidates(): void {
    this.router.navigate(['/admin/candidates']);
  }

  goToUsers(): void {
    this.router.navigate(['/admin/users']);
  }

  goHome(): void {
    // landing donde el usuario puede elegir Iniciar sesión / Registrarme
    this.router.navigate(['/']);
  }

  logout(): void {
    this.authService.logout(); // limpia token, etc.
    this.router.navigate(['/']); // vuelve al landing
  }
}



