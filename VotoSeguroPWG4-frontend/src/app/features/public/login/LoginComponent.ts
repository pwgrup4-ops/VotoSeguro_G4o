import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { LoginDto } from '../../../shared/models/user.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterModule, CommonModule],
  templateUrl: './LoginComponent.html',
  styleUrls: ['./LoginComponent.css']
})
export class LoginComponent {
  form: LoginDto = { email: '', password: '' };
  loading = false;
  error: string | null = null;

  emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;


  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!this.form.email || !this.form.password) {
      this.error = 'Completa todos los campos';
      return;
    }

    if (!emailRegex.test(this.form.email)) {
      this.error = 'Email inválido';
      return;
    }

    if (this.form.password.length < 8) {
      this.error = 'La contraseña debe tener al menos 8 caracteres';
      return;
    }

    this.loading = true;
    this.error = null;

    this.authService.login(this.form).subscribe({
      next: () => {
        this.loading = false;
        const role = this.authService.getUserRole();
        if (role === 'admin') {
          this.router.navigate(['/admin/dashboard']);
        } else {
          this.router.navigate(['/voter/candidates']);
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error ?? 'Error al iniciar sesión';
      }
    });
  }

}
