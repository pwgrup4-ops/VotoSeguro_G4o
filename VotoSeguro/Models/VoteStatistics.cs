
using System.Collections.Generic;


namespace VotoSeguro.Models
{
    public class VoteStatistics
    {
        public int TotalRegisteredVoters { get; set; }
        public int TotalVotesCast { get; set; }
        public double ParticipationRate { get; set; } // Porcentaje de participación [cite: 30]
        public List<CandidateResult> CandidateResults { get; set; } = new List<CandidateResult>();
        public List<VoterStatusCount> VoterStatuses { get; set; } = new List<VoterStatusCount>();
    }

    public class CandidateResult
    {
        public string CandidateId { get; set; } = string.Empty;
        public string CandidateName { get; set; } = string.Empty;
        public string Party { get; set; } = string.Empty;
        public int VoteCount { get; set; }
        public double VotePercentage { get; set; }
    }

    public class VoterStatusCount
    {
        public string Status { get; set; } = string.Empty; // 'Votó' o 'Pendiente' [cite: 35]
        public int Count { get; set; }
    }
}