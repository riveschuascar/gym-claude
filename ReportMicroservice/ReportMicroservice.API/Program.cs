using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ReportMicroservice.Application.Services;
using ReportMicroservice.Domain.Ports;
using ReportMicroservice.Infrastructure.Reports;
using ReportMicroservice.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Inyección de HttpClient para llamadas externas
builder.Services.AddHttpClient<IExternalDataService, ExternalDataService>();

// 2. Inyección de dependencias del Proyecto
builder.Services.AddScoped<IReportBuilder, PdfReportBuilder>();
builder.Services.AddScoped<ReportGenerationService>();

// 3. HttpContextAccessor para leer el JWT y Claims
builder.Services.AddHttpContextAccessor();

// Configuración básica de autenticación (para que funcione User.Claims)
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Importante para leer los claims
app.UseAuthorization();

app.MapControllers();

app.Run();