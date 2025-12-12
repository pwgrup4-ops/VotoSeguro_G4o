
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VotoSeguro.Services; // Importar el namespace de los servicios

var builder = WebApplication.CreateBuilder(args);

// =================================================================
// 1. REGISTRAR SERVICIOS (Inyección de Dependencias)
// =================================================================

// Firebase Services (Singleton)
builder.Services.AddSingleton<FirebaseServices>(); 

// Servicios de Lógica (Scoped)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Configurar CORS para el Frontend Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        // El frontend Angular típicamente corre en el puerto 4200
        policy.WithOrigins("http://localhost:4200") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// =================================================================
// 2. CONFIGURAR AUTENTICACIÓN JWT
// =================================================================
var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? throw new InvalidOperationException("JWT Key no configurada en appsettings.json");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero 
    };
});

builder.Services.AddAuthorization(); // Registrar el servicio de autorización

// =================================================================
// 3. CONFIGURACIÓN BASE
// =================================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// EL ORDEN DE LOS MIDDLEWARE ES CRÍTICO:
app.UseRouting(); // Necesario para CORS en algunos casos
app.UseCors("AllowAngular"); // CORS debe ir antes de Auth/Authz
app.UseAuthentication();    // Verifica el token
app.UseAuthorization();     // Verifica los roles y políticas

app.MapControllers();

app.Run();