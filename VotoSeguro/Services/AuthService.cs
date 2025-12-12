

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using VotoSeguro.DTOs;
using VotoSeguro.Models;

namespace VotoSeguro.Services
{
    public class AuthService : IAuthService
    {
        private readonly FirebaseServices _firebaseService;
        private readonly IConfiguration _configuration;

        public AuthService(FirebaseServices firebaseService, IConfiguration configuration)
        {
            _firebaseService = firebaseService;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> Register(RegisterDto registerdto)
        {
            try
            {
                var existingUser = await GetUserByEmail(registerdto.Email);
                if (existingUser != null)
                {
                    throw new Exception("Ya existe un usuario con este email.");
                }
                
                var userId = Guid.NewGuid().ToString();
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerdto.Password);
                
                // Rol por defecto: "votante"
                var user = new User
                {
                    Id = userId,
                    Email = registerdto.Email,
                    Fullname = registerdto.FullName,
                    Role = "votante", 
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    HasVoted = false 
                };
                
                var usersCollection = _firebaseService.GetCollection("users");
                
                var userData = new Dictionary<string, object>()
                {
                    {"Id", user.Id},
                    {"Email", user.Email},
                    {"Fullname", user.Fullname},
                    {"Role", user.Role},
                    {"CreatedAt", user.CreatedAt},
                    {"IsActive", user.IsActive},
                    {"PasswordHash", passwordHash},
                    {"HasVoted", user.HasVoted},
                    {"VotedForCandidateId", null},
                    {"VotedForCandidateName", null},
                    {"VoteTimestamp", null}
                };
                await usersCollection.Document(user.Id).SetAsync(userData);
                
                var token = GenerateJwtToken(user);
                
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.Fullname,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al registrar usuario: {ex.Message}");
            }
        }

        // Login (Reutilizada del proyecto anterior, solo asegurarse que maneja el campo PasswordHash)
        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            // Lógica completa de obtención de usuario, verificación de contraseña con BCrypt y generación de token JWT
            try
            {
                var userCollection = _firebaseService.GetCollection("users");
                var query = userCollection.WhereEqualTo("Email", loginDto.Email).Limit(1);
                var snapshot = await query.GetSnapshotAsync();
                
                if (snapshot.Count == 0)
                {
                    throw new Exception("Credenciales inválidas");
                }
                
                var userDoc = snapshot.Documents[0];
                var userData = userDoc.ToDictionary();
                
                if (!userData.ContainsKey("PasswordHash"))
                {
                    throw new Exception("Usuario sin contraseña configurada.");
                }
                
                var passwordHash = userData["PasswordHash"].ToString();
                
                // Convertir el documento a User, incluyendo los nuevos campos
                var user = userDoc.ConvertTo<User>(); 
                
                if (string.IsNullOrEmpty(passwordHash) || !BCrypt.Net.BCrypt.Verify(loginDto.Password, passwordHash))
                {
                    throw new Exception("Credenciales inválidas");
                }
                
                if (!user.IsActive)
                {
                    throw new Exception("Usuario inactivo");
                }
                
                var token = GenerateJwtToken(user);
                
                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.Fullname,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al iniciar sesión: {ex.Message}");
            }
        }

        // GetUserById (Reutilizada del proyecto anterior)
        public async Task<User?> GetUserById(string userId)
        {
            try
            {
                var userDoc = await _firebaseService
                    .GetCollection("users")
                    .Document(userId)
                    .GetSnapshotAsync();
                
                return userDoc.Exists ? userDoc.ConvertTo<User>() : null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        // GetUserByEmail (Reutilizada del proyecto anterior)
        public async Task<User?> GetUserByEmail(string email)
        {
            try
            {
                var userCollection = _firebaseService.GetCollection("users");
                var query = userCollection.WhereEqualTo("Email", email).Limit(1);
                var snapshot = await query.GetSnapshotAsync();
                
                if (snapshot.Count == 0)
                {
                    return null;
                }
                
                return snapshot.Documents[0].ConvertTo<User>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GenerateJwtToken(User user)
        {
            // Lógica completa de generación de token (reutilizada)
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key no configurado"); 
            var jwtIssuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer no configurado");
            var jwtAudience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience no configurado");
            var jwtExpiryInMinutes = int.Parse(_configuration["Jwt:ExpireInMinutes"] ?? "60");
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Fullname),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpiryInMinutes),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<List<User>> GetAllVoters()
        {
            try
            {
                var usersCollection = _firebaseService.GetCollection("users");
                var snapshot = await usersCollection.GetSnapshotAsync();

                var users = new List<User>();
                foreach (var document in snapshot.Documents)
                {
                    // No incluimos el PasswordHash si usamos ConvertTo<User> porque no está en el modelo
                    users.Add(document.ConvertTo<User>());
                }

                return users.Where(u => u.Role == "votante").ToList();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }
    }
}