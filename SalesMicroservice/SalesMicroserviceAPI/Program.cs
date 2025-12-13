using System.Text;
using SalesMicroserviceApplication.Interfaces;
using SalesMicroserviceApplication.Services;
using SalesMicroserviceInfraestructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SalesMicroserviceAPI.Http;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using SalesMicroserviceDomain.Ports;
using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<PropagationDelegatingHandler>();

var httpTimeout = builder.Configuration.GetValue<int?>("ExternalApis:TimeoutSeconds") ?? 5;
var clientApiBase = builder.Configuration["ExternalApis:Client"] ?? "http://localhost:5135";
var disciplineApiBase = builder.Configuration["ExternalApis:Discipline"] ?? builder.Configuration["ExternalApis:Membership"] ?? "http://localhost:5292";

builder.Services.AddHttpClient("Clients", client =>
{
    client.BaseAddress = new Uri(clientApiBase);
    client.Timeout = TimeSpan.FromSeconds(httpTimeout);
}).AddHttpMessageHandler<PropagationDelegatingHandler>();

builder.Services.AddHttpClient("Disciplines", client =>
{
    client.BaseAddress = new Uri(disciplineApiBase);
    client.Timeout = TimeSpan.FromSeconds(httpTimeout);
}).AddHttpMessageHandler<PropagationDelegatingHandler>();

builder.Services.AddScoped<IDbConnection>(_ =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("SalesMicroserviceDB")));
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
// outbox not used for now
builder.Services.AddScoped<ISaleService, SaleService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
