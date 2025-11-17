using DisciplineMicroservice.DisciplineMicroserviceApplication.Interfaces;
using DisciplineMicroservice.DisciplineMicroserviceApplication.Services;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Ports;
using DisciplineMicroservice.DisciplineMicroserviceInfraestructure.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDisciplineRepository, DisciplineRepository>();
builder.Services.AddScoped<IDisciplineService, DisciplineService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
