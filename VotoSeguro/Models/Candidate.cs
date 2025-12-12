

using Google.Cloud.Firestore;
using System.Collections.Generic;


namespace VotoSeguro.Models
{
    [FirestoreData]
    public class Candidate
    {
        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Name { get; set; } = string.Empty;
        
        [FirestoreProperty]
        public string Party { get; set; } = string.Empty; // Nombre del partido [cite: 24]
        
        [FirestoreProperty]
        public string PhotoUrl { get; set; } = string.Empty; // URL de la foto en Storage 
        
        [FirestoreProperty]
        public string LogoUrl { get; set; } = string.Empty; // URL del logo del partido en Storage 
        
        [FirestoreProperty]
        public List<string> Proposals { get; set; } = new List<string>(); // Propuestas [cite: 24]
        
        [FirestoreProperty]
        public int VoteCount { get; set; } = 0; // Contador de votos (para el reporte) 
        
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Para rastreo [cite: 98]

        [FirestoreProperty]
        public string CreatedByUserId { get; set; } = string.Empty; // Para rastreo [cite: 98]

        [FirestoreProperty]
        public bool IsActive { get; set; } = true; // Permite desactivar sin eliminar

    }
}