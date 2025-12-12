


using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotoSeguro.Services;

namespace VotoSeguro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")] // Estrictamente limitado al rol admin
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: api/Users/voters
        // Obtiene la lista de todos los usuarios registrados como votantes
        [HttpGet("voters")]
        public async Task<IActionResult> GetAllVoters()
        {
            try
            {
                var voters = await _authService.GetAllVoters();
                
                // Proyectar para evitar exponer PasswordHash
                var response = voters.Select(v => new 
                {
                    v.Id,
                    v.Email,
                    v.Fullname,
                    v.Role,
                    v.IsActive,
                    v.CreatedAt,
                    // Campos de auditoría de voto
                    v.HasVoted,
                    v.VotedForCandidateId,
                    v.VotedForCandidateName,
                    v.VoteTimestamp
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        // La ruta GET: api/Users/{id} de auditoría ya está cubierta por api/Auth/me si es Admin.
        // Se podría agregar una ruta para obtener un votante específico para el Admin si fuera necesario, 
        // pero GetAllVoters provee la mayoría de la información de auditoría.
    }
}