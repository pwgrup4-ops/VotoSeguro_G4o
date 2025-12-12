

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VotoSeguro.DTOs
{
    public class CandidateDto
    {
        [Required(ErrorMessage = "El nombre del candidato es requerido")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nombre del partido es requerido")]
        [MaxLength(100)]
        public string Party { get; set; } = string.Empty;
        
        // Asumimos que la URL es enviada después de la subida a Firebase Storage
        [Required(ErrorMessage = "La URL de la foto es requerida")]
        [Url(ErrorMessage = "Formato de URL inválido")]
        public string PhotoUrl { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La URL del logo es requerida")]
        [Url(ErrorMessage = "Formato de URL inválido")]
        public string LogoUrl { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Debe incluir al menos una propuesta")]
        public List<string> Proposals { get; set; } = new List<string>();
    }
}