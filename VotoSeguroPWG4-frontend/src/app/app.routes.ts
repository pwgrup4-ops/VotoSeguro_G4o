
// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { LandingComponent } from './features/public/landing/LandingComponent';
import { LoginComponent } from './features/public/login/LoginComponent';
import { RegisterComponent } from './features/public/register/RegisterComponent';

// Votante
import { CandidatesListComponent } from './features/user/candidates-list/CandidatesListComponent';
import { VotingComponent } from './features/user/voting/VotingComponent';
import { VoteConfirmationComponent } from './features/user/vote-confirmation/VoteConfirmationComponent';
import { AlreadyVotedComponent } from './features/user/already-voted/AlreadyVotedComponent';
import { VoterDashboardComponent } from './features/user/voter-dashboard/voter-dashboard'; // ← NUEVO

// Admin
import { DashboardComponent } from './features/admin/dashboard/DashboardComponent';
import { ReportsComponent } from './features/admin/reports/ReportsComponent';
import { UserListComponent } from './features/admin/user-list/UserListComponent';
import { CandidatesListComponent as AdminCandidatesListComponent } from './features/admin/candidate-list/CandidatesListComponent';
import { CandidateFormComponent } from './features/admin/candidate-form/CandidateFormComponent';

// Guards
import { authGuard } from './core/guards/auth-guard';
import { alreadyVotedGuard } from './core/guards/already-voted.guard';

export const routes: Routes = [
  { path: '', component: LandingComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Rutas votante
  {
    path: 'voter',
    canActivate: [authGuard],
    children: [
      { path: 'candidates', component: CandidatesListComponent, canActivate: [alreadyVotedGuard] },
      { path: 'voting/:id', component: VotingComponent, canActivate: [alreadyVotedGuard] },
      { path: 'vote-confirmation', component: VoteConfirmationComponent },
      { path: 'already-voted', component: AlreadyVotedComponent },
      { path: 'dashboard', component: VoterDashboardComponent } // ← NUEVA
    ]
  },

  // Rutas admin
  {
    path: 'admin',
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'reports', component: ReportsComponent },
      { path: 'users', component: UserListComponent },
      { path: 'candidates', component: AdminCandidatesListComponent },
      { path: 'candidates/new', component: CandidateFormComponent },
      { path: 'candidates/:id/edit', component: CandidateFormComponent }
    ]
  },

  { path: '**', redirectTo: '' }
];

