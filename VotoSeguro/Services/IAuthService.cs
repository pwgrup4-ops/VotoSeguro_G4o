



using VotoSeguro.DTOs;
using VotoSeguro.Models;

namespace VotoSeguro.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Register(RegisterDto registerdto);
        Task<AuthResponseDto> Login(LoginDto logindto);
        Task<User?> GetUserById(string userId);
        Task<User?> GetUserByEmail(string email);
        string GenerateJwtToken(User user);
        
        // Nuevo método para auditoría (solo lectura)
        Task<List<User>> GetAllVoters();
    }
}