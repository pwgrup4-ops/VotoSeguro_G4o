



using VotoSeguro.DTOs;
using VotoSeguro.Models;

namespace VotoSeguro.Services
{
    public interface IVoteService
    {
        // El método central que registra el voto y realiza la validación de voto único
        Task<Vote> CastVote(string userId, VoteDto voteDto);
        Task<User?> GetUserVoteStatus(string userId);
    }
}