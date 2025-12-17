

import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { VoteService } from '../services/VoteService';
import { map, catchError, of } from 'rxjs';

export const alreadyVotedGuard: CanActivateFn = () => {
  const voteService = inject(VoteService);
  const router = inject(Router);

  return voteService.getStatus().pipe(
    map(status => {
      if (status.hasVoted) {
        router.navigate(['/voter/already-voted'], {
          state: {
            candidateName: status.votedForCandidateName,
            timestamp: status.voteTimestamp
          }
        });
        return false;
      }
      return true;
    }),
    catchError(() => of(true))
  );
};
