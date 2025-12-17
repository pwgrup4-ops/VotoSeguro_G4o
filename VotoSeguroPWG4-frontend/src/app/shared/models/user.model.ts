
export interface User {
  id: string;
  email: string;
  fullname: string;
  role: 'admin' | 'votante';
  hasVoted?: boolean;
  votedForCandidateName?: string | null;
  voteTimestamp?: string | null;
}

export interface RegisterDto {
  email: string;
  password: string;
  fullname: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
}
