
export interface Candidate {
  id: string;
  name: string;
  party: string;
  photoUrl?: string;
  logoUrl?: string;
  proposals?: string[];
  voteCount?: number;
}

