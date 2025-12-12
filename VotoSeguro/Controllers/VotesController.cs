// File: Controllers/VotesController.cs

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotoSeguro.DTOs;
using VotoSeguro.Services;

namespace VotoSeguro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "votante")] // Solo los votantes pueden acceder a estas rutas
    public class VotesController : ControllerBase
    {
        private readonly IVoteService _voteService;
        private readonly IAuthService _authService;

        public VotesController(IVoteService voteService, IAuthService authService)
        {
            _voteService = voteService;
            _authService = authService;
        }

        // POST: api/Votes/cast
        // Endpoint para emitir el voto (usando la Transacción de Firestore)
        [HttpPost("cast")]
        public async Task<IActionResult> CastVote([FromBody] VoteDto voteDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { error = "Token inválido" });
                
                // El servicio maneja la lógica de Voto Único
                var voteRecord = await _voteService.CastVote(userId, voteDto);
                
                return Ok(new 
                {
                    message = "Voto registrado exitosamente. Gracias por participar.",
                    voteRecord.CandidateName,
                    voteRecord.Timestamp
                });
            }
            catch (Exception ex)
            {
                // Maneja errores específicos como "El usuario ya ha emitido su voto."
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Votes/status
        // Permite al votante verificar si ya votó
        [HttpGet("status")]
        public async Task<IActionResult> GetVoteStatus()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { error = "Token inválido" });

                var user = await _authService.GetUserById(userId);
                
                if (user == null) 
                    return NotFound(new { error = "Usuario no encontrado" });

                return Ok(new
                {
                    user.HasVoted,
                    user.VotedForCandidateName,
                    user.VoteTimestamp
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}