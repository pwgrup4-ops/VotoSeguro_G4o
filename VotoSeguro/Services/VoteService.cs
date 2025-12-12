


using Google.Cloud.Firestore;
using VotoSeguro.DTOs;
using VotoSeguro.Models;

namespace VotoSeguro.Services
{
    public class VoteService : IVoteService
    {
        private readonly FirebaseServices _firebaseService;
        private readonly IAuthService _authService;
        private readonly ICandidateService _candidateService;

        public VoteService(FirebaseServices firebaseService, IAuthService authService, ICandidateService candidateService)
        {
            _firebaseService = firebaseService;
            _authService = authService;
            _candidateService = candidateService;
        }

        public async Task<Vote> CastVote(string userId, VoteDto voteDto)
        {
            var db = _firebaseService.GetFirestoreDb();
            var userRef = _firebaseService.GetCollection("users").Document(userId);
            var candidateRef = _firebaseService.GetCollection("Candidates").Document(voteDto.CandidateId);
            
            // 1. Verificar existencia del candidato (fuera de la transacción para fail fast)
            var candidate = await _candidateService.GetCandidateById(voteDto.CandidateId);
            if (candidate == null)
            {
                throw new Exception("Candidato no encontrado.");
            }

            // 2. Ejecutar la Transacción de Firestore (Garantiza Voto Único)
            var voteResult = await db.RunTransactionAsync(async transaction =>
            {
                // Obtener el estado actual del usuario
                var userSnapshot = await transaction.GetSnapshotAsync(userRef);

                if (!userSnapshot.Exists)
                {
                    throw new Exception("Votante no encontrado.");
                }

                var user = userSnapshot.ConvertTo<User>();
                
                // VALIDACIÓN CLAVE: Bloqueo permanente si ya votó
                if (user.HasVoted)
                {
                    throw new Exception("El usuario ya ha emitido su voto. Voto Bloqueado.");
                }

                var timestamp = DateTime.UtcNow;
                
                // 3. Crear el registro de Voto (Auditoría, colección Votes)
                var newVote = new Vote
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    UserName = user.Fullname,
                    CandidateId = candidate.Id,
                    CandidateName = candidate.Name,
                    Timestamp = timestamp
                };
                
                var voteRef = _firebaseService.GetCollection("Votes").Document(newVote.Id);
                transaction.Set(voteRef, newVote);
                
                // 4. Actualizar el estado del usuario (colección Users)
                var userUpdates = new Dictionary<string, object>
                {
                    { "HasVoted", true },
                    { "VotedForCandidateId", candidate.Id },
                    { "VotedForCandidateName", candidate.Name },
                    { "VoteTimestamp", timestamp }
                };
                transaction.Update(userRef, userUpdates);

                // 5. Incrementar contador del candidato (colección Candidates)
                transaction.Update(candidateRef, "VoteCount", FieldValue.Increment(1));

                return newVote;
            });

            return voteResult;
        }

        public async Task<User?> GetUserVoteStatus(string userId)
        {
            // Reutiliza GetUserById de AuthService para obtener el estado de voto
            return await _authService.GetUserById(userId);
        }
    }
}