
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { UserService } from '../../../core/services/user'; // o user.service si lo renombraste
import { User } from '../../../shared/models/user.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './UserListComponent.html',
  styleUrls: ['./UserListComponent.css'],
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  filtered: User[] = [];
  loading = true;
  error: string | null = null;

  search = '';
  statusFilter: 'all' | 'voted' | 'notVoted' = 'all';

  constructor(
    private userService: UserService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.error = null;
    this.cdr.markForCheck();

    this.userService.getVoters().subscribe({
      next: (data) => {
        this.users = data || [];
        this.applyFilters();
        this.loading = false;
        this.cdr.detectChanges();   // fuerza actualización de la vista
      },
      error: (err) => {
        console.error('Error al cargar votantes', err);
        this.error = 'No se pudieron cargar los votantes.';
        this.users = [];
        this.filtered = [];
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  applyFilters(): void {
    const term = this.search.toLowerCase().trim();

    this.filtered = this.users.filter((u) => {
      const matchesSearch =
        !term ||
        u.fullname.toLowerCase().includes(term) ||
        u.email.toLowerCase().includes(term);

      const matchesStatus =
        this.statusFilter === 'all' ||
        (this.statusFilter === 'voted' && u.hasVoted) ||
        (this.statusFilter === 'notVoted' && !u.hasVoted);

      return matchesSearch && matchesStatus;
    });
  }

  onSearchChange(value: string): void {
    this.search = value;
    this.applyFilters();
    this.cdr.detectChanges();
  }

  onStatusChange(value: 'all' | 'voted' | 'notVoted'): void {
    this.statusFilter = value;
    this.applyFilters();
    this.cdr.detectChanges();
  }

  goBack(): void {
    this.router.navigate(['/admin/dashboard']);
  }
}

