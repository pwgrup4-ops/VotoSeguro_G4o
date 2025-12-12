


using Google.Cloud.Firestore;
using VotoSeguro.Models;

namespace VotoSeguro.Services
{
    public class ReportService : IReportService
    {
        private readonly FirebaseServices _firebaseService;
        private readonly ICandidateService _candidateService;
        private readonly IAuthService _authService;

        public ReportService(FirebaseServices firebaseService, ICandidateService candidateService, IAuthService authService)
        {
            _firebaseService = firebaseService;
            _candidateService = candidateService;
            _authService = authService;
        }

        public async Task<VoteStatistics> GetVoteStatistics()
        {
            try
            {
                // 1. Obtener datos necesarios
                var allVoters = await _authService.GetAllVoters();
                var allCandidates = await _candidateService.GetAllCandidates();
                
                int totalRegisteredVoters = allVoters.Count;
                int totalVotesCast = allVoters.Count(u => u.HasVoted);

                var statistics = new VoteStatistics
                {
                    TotalRegisteredVoters = totalRegisteredVoters,
                    TotalVotesCast = totalVotesCast,
                };
                
                // 2. Calcular tasa de participación
                if (totalRegisteredVoters > 0)
                {
                    statistics.ParticipationRate = Math.Round(
                        (double)statistics.TotalVotesCast / totalRegisteredVoters * 100, 2
                    );
                }

                // 3. Resultados por candidato (Gráfico de barras)
                statistics.CandidateResults = allCandidates
                    .Select(c => new CandidateResult
                    {
                        CandidateId = c.Id,
                        CandidateName = c.Name,
                        Party = c.Party,
                        VoteCount = c.VoteCount,
                        VotePercentage = totalVotesCast > 0 
                            ? Math.Round((double)c.VoteCount / totalVotesCast * 100, 2)
                            : 0
                    })
                    .OrderByDescending(r => r.VoteCount)
                    .ToList();

                // 4. Estado de votantes (Gráfico circular)
                statistics.VoterStatuses.Add(new VoterStatusCount
                {
                    Status = "Votó",
                    Count = totalVotesCast
                });
                statistics.VoterStatuses.Add(new VoterStatusCount
                {
                    Status = "Pendiente",
                    Count = totalRegisteredVoters - totalVotesCast
                });

                return statistics;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas de votación: {ex.Message}");
            }
        }
        
        public async Task<List<Vote>> GetVoteAuditLog()
        {
            try
            {
                var votesCollection = _firebaseService.GetCollection("Votes");
                var snapshot = await votesCollection.GetSnapshotAsync();

                var votes = snapshot.Documents.Select(d => d.ConvertTo<Vote>()).ToList();
                
                return votes.OrderByDescending(v => v.Timestamp).ToList();
            }
            catch (Exception)
            {
                return new List<Vote>();
            }
        }
    }
}