import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { RegisterDto } from '../../../shared/models/user.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterModule, CommonModule],
  templateUrl: './RegisterComponent.html',
  styleUrls: ['./RegisterComponent.css']
})
export class RegisterComponent {
  form: RegisterDto = { email: '', password: '', fullname: '' };
  loading = false;
  error: string | null = null;

  emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

  get isInvalidForm(): boolean {
    return (
      !this.form.fullname ||
      !this.form.email ||
      !this.form.password ||
      !this.emailRegex.test(this.form.email) ||
      this.form.password.length < 8
    );
  }



  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!this.form.fullname || !this.form.email || !this.form.password) {
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

    this.authService.register(this.form).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error ?? 'Error al registrarse';
      }
    });
  }

}
