using ClientMicroservice.Application.Services;
using ClientMicroservice.Domain.Interfaces;
using ClientMicroservice.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ClientService>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
