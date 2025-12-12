
using System.ComponentModel.DataAnnotations;

namespace VotoSeguro.DTOs
{
    public class VoteDto
    {
        [Required(ErrorMessage = "El ID del candidato es requerido")]
        public string CandidateId { get; set; } = string.Empty;
    }
}