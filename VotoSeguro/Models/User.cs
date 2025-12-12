


using Google.Cloud.Firestore;

namespace VotoSeguro.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Email { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Fullname { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Role { get; set; } = "votante"; // Rol por defecto es 'votante'
        
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        // Nuevos campos para la votación
        [FirestoreProperty]
        public bool HasVoted { get; set; } = false; // Indica si ya votó [cite: 95]
        
        [FirestoreProperty]
        public string? VotedForCandidateId { get; set; } // ID del candidato por quien votó [cite: 95]
        
        [FirestoreProperty]
        public string? VotedForCandidateName { get; set; } // Nombre del candidato por quien votó [cite: 95]
        
        [FirestoreProperty]
        public DateTime? VoteTimestamp { get; set; } // Fecha y hora del voto para auditoría [cite: 96]
    }
}