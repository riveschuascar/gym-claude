using UserMicroservice.Application.Services;
using UserMicroservice.Domain.Ports;
using UserMicroservice.Infrastructure.Persistence;
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
