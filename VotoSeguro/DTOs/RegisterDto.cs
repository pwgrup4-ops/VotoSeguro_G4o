
using System.ComponentModel.DataAnnotations;

namespace VotoSeguro.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El email es inválido")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El password es requerido")]
        [MinLength(6, ErrorMessage = "El password debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nombre completo es requerido")]
        public string FullName { get; set; } = string.Empty;
    }
}