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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
