


using VotoSeguro.DTOs;
using VotoSeguro.Models;

namespace VotoSeguro.Services
{
    public interface ICandidateService
    {
        Task<Candidate> CreateCandidate(CandidateDto candidateDto, string createdByUserId);
        Task<Candidate?> GetCandidateById(string candidateId);
        Task<List<Candidate>> GetAllCandidates();
        Task<Candidate?> UpdateCandidate(string candidateId, CandidateDto candidateDto);
        Task<bool> DeleteCandidate(string candidateId);
        
        // Método utilizado internamente por VoteService
        Task IncrementVoteCount(string candidateId);
    }
}