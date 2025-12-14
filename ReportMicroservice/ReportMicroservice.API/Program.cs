using ReportMicroservice.Application.Services;
using ReportMicroservice.Domain.Ports;
using ReportMicroservice.Infrastructure.Reports;
using ReportMicroservice.Infrastructure.Services;

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
// Necesitas el paquete Microsoft.AspNetCore.Authentication.JwtBearer
/*
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        // Configura aquí tu Authority o Validación de Token igual que tus otros microservicios
        options.Authority = "https://tu-auth-server";
        options.Audience = "report-api";
    });
*/

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