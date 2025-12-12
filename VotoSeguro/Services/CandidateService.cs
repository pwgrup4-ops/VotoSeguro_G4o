
// File: Services/CandidateService.cs

using Google.Cloud.Firestore;
using VotoSeguro.DTOs;
using VotoSeguro.Models;

namespace VotoSeguro.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly FirebaseServices _firebaseService;
        
        public CandidateService(FirebaseServices firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task<Candidate> CreateCandidate(CandidateDto candidateDto, string createdByUserId)
        {
            try
            {
                var candidateId = Guid.NewGuid().ToString();
                var candidate = new Candidate
                {
                    Id = candidateId,
                    Name = candidateDto.Name,
                    Party = candidateDto.Party,
                    PhotoUrl = candidateDto.PhotoUrl,
                    LogoUrl = candidateDto.LogoUrl,
                    Proposals = candidateDto.Proposals,
                    CreatedByUserId = createdByUserId,
                    CreatedAt = DateTime.UtcNow,
                    VoteCount = 0
                };
                
                var candidatesCollection = _firebaseService.GetCollection("Candidates");
                await candidatesCollection.Document(candidateId).SetAsync(candidate);
                
                return candidate;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear candidato: {ex.Message}");
            }
        }

        public async Task<List<Candidate>> GetAllCandidates()
        {
            try
            {
                var candidatesCollection = _firebaseService.GetCollection("Candidates");
                var snapshot = await candidatesCollection
                    .WhereEqualTo("IsActive", true) // Solo candidatos activos
                    .GetSnapshotAsync();
                
                return snapshot.Documents.Select(d => d.ConvertTo<Candidate>()).ToList();
            }
            catch (Exception)
            {
                return new List<Candidate>();
            }
        }
        
        public async Task<Candidate?> GetCandidateById(string candidateId)
        {
             try
            {
                var candidateDoc = await _firebaseService
                    .GetCollection("Candidates")
                    .Document(candidateId)
                    .GetSnapshotAsync();

                return candidateDoc.Exists ? candidateDoc.ConvertTo<Candidate>() : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public async Task<Candidate?> UpdateCandidate(string candidateId, CandidateDto candidateDto)
        {
            try
            {
                var docRef = _firebaseService.GetCollection("Candidates").Document(candidateId);
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists)
                {
                    return null;
                }
                
                var updates = new Dictionary<string, object>
                {
                    { "Name", candidateDto.Name },
                    { "Party", candidateDto.Party },
                    { "PhotoUrl", candidateDto.PhotoUrl },
                    { "LogoUrl", candidateDto.LogoUrl },
                    { "Proposals", candidateDto.Proposals }
                };

                await docRef.UpdateAsync(updates);
                var updatedSnapshot = await docRef.GetSnapshotAsync();
                return updatedSnapshot.ConvertTo<Candidate>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar candidato: {ex.Message}");
            }
        }

        public async Task<bool> DeleteCandidate(string candidateId)
        {
            try
            {
                var docRef = _firebaseService.GetCollection("Candidates").Document(candidateId);
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists) return false;
                
                var candidate = snapshot.ConvertTo<Candidate>();
                
                // Validación: No se puede eliminar un candidato que ya tiene votos
                if (candidate.VoteCount > 0)
                {
                    throw new Exception("No se puede eliminar un candidato que ya tiene votos.");
                }

                await docRef.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Incluir mensaje de la excepción si es la de 'No se puede eliminar'
                throw new Exception($"Error al eliminar candidato: {ex.Message}");
            }
        }
        
        public async Task IncrementVoteCount(string candidateId)
        {
            var docRef = _firebaseService.GetCollection("Candidates").Document(candidateId);
            // Incremento atómico (utilizado en la transacción de VoteService)
            await docRef.UpdateAsync("VoteCount", FieldValue.Increment(1));
        }
    }
}