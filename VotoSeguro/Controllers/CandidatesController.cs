



using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotoSeguro.DTOs;
using VotoSeguro.Services;

namespace VotoSeguro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere autenticación para cualquier operación
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidatesController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        // GET: api/Candidates
        // Permitido para Votantes y Admin (para ver a quién votar/gestionar)
        [HttpGet]
        public async Task<IActionResult> GetAllCandidates()
        {
            try
            {
                var candidates = await _candidateService.GetAllCandidates();
                return Ok(candidates);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        // GET: api/Candidates/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCandidateById(string id)
        {
            try
            {
                var candidate = await _candidateService.GetCandidateById(id);
                if (candidate == null) return NotFound(new { error = "Candidato no encontrado" });
                
                return Ok(candidate);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/Candidates (Solo Admin)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateCandidate([FromBody] CandidateDto candidateDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId)) 
                    return Unauthorized(new { error = "Token inválido" });
                
                var candidate = await _candidateService.CreateCandidate(candidateDto, userId);
                return CreatedAtAction(nameof(GetCandidateById), new { id = candidate.Id }, candidate);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT: api/Candidates/{id} (Solo Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCandidate(string id, [FromBody] CandidateDto candidateDto)
        {
            try
            {
                var updatedCandidate = await _candidateService.UpdateCandidate(id, candidateDto);

                if (updatedCandidate == null) 
                    return NotFound(new { error = "Candidato no encontrado" });

                return Ok(updatedCandidate);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE: api/Candidates/{id} (Solo Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCandidate(string id)
        {
            try
            {
                var deleted = await _candidateService.DeleteCandidate(id);

                if (!deleted) 
                    return NotFound(new { error = "Candidato no encontrado o ya tiene votos (no se puede eliminar)" });

                return NoContent(); // 204 No Content para eliminación exitosa
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}