

using Google.Cloud.Firestore;

namespace VotoSeguro.Models
{
    [FirestoreData]
    public class Vote
    {
        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string UserId { get; set; } = string.Empty; // Quién votó 
        
        [FirestoreProperty]
        public string UserName { get; set; } = string.Empty; 
        
        [FirestoreProperty]
        public string CandidateId { get; set; } = string.Empty; // Por quién votó 
        
        [FirestoreProperty]
        public string CandidateName { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Cuándo votó 
    }
}