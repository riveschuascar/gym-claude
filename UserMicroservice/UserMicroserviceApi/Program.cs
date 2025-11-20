using UserMicroservice.Application.Services;
using UserMicroservice.Domain.Ports;
using UserMicroservice.Infrastructure.Persistence;
using EmailMicroservice.API;
using UserMicroservice.Api.Services;
using System.Net.Http;
using System;
using Grpc.Net.ClientFactory;
using Npgsql;
using System.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configuración de conexión PostgreSQL
builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias de capas
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

// gRPC client for EmailService (configured via appsettings EmailService:Url)
var emailServiceUrl = builder.Configuration["EmailService:Url"] ?? "https://localhost:50051";
if (Uri.TryCreate(emailServiceUrl, UriKind.Absolute, out var emailUri))
{
    if (emailUri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
    {
        // Allow unencrypted HTTP/2 for development/local servers that don't use TLS
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        builder.Services.AddGrpcClient<EmailService.EmailServiceClient>(options =>
        {
            options.Address = emailUri;
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());
    }
    else
    {
        builder.Services.AddGrpcClient<EmailService.EmailServiceClient>(options =>
        {
            options.Address = emailUri;
        });
    }
}
else
{
    builder.Services.AddGrpcClient<EmailService.EmailServiceClient>(options =>
    {
        options.Address = new Uri("https://localhost:50051");
    });
}

builder.Services.AddScoped<IEmailClient, EmailGrpcClient>();

// Agregar controladores
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
