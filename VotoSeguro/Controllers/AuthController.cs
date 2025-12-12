

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotoSeguro.DTOs; // Adaptar namespace
using VotoSeguro.Services; // Adaptar namespace

namespace VotoSeguro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        // Registro de un nuevo votante (rol por defecto)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var response = await _authService.Register(registerDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var response = await _authService.Login(loginDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/Auth/me
        // Devuelve la información del usuario logueado
        [HttpGet("me")]
        [Authorize] 
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "Token no válido o expirado" });
                }

                var user = await _authService.GetUserById(userId);
                if (user == null)
                {
                    return NotFound(new { error = "Usuario no encontrado" });
                }
                
                // Excluir información sensible
                return Ok(new 
                {
                    user.Id,
                    user.Email,
                    user.Fullname,
                    user.Role,
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