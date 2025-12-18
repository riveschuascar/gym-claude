using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Orchestrator.Application.Interfaces;
using Orchestrator.Application.Services;
using Orchestrator.Rest.Http;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Configuration
// --------------------
builder.Configuration
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                    optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();

// --------------------
// Services
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpContext (útil si luego propagas token)
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<PropagationDelegatingHandler>();

// --------------------
// HttpClientFactory
// --------------------

builder.Services.AddHttpClient("clients", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:clients:BaseUrl"] ?? "");
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<PropagationDelegatingHandler>();

builder.Services.AddHttpClient("disciplines", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:disciplines:BaseUrl"] ?? "");
    client.Timeout = TimeSpan.FromSeconds(60);
}).AddHttpMessageHandler<PropagationDelegatingHandler>();

builder.Services.AddHttpClient("sales", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:sales:BaseUrl"] ?? "");
    client.Timeout = TimeSpan.FromSeconds(60);
}).AddHttpMessageHandler<PropagationDelegatingHandler>();

builder.Services.AddHttpClient("saleDetails", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:sale_details:BaseUrl"] ?? "");
    client.Timeout = TimeSpan.FromSeconds(60);
}).AddHttpMessageHandler<PropagationDelegatingHandler>();

builder.Services.AddHttpClient("reports", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:reports:BaseUrl"] ?? "");
    client.Timeout = TimeSpan.FromSeconds(60);
}).AddHttpMessageHandler<PropagationDelegatingHandler>();

// --------------------
// Registrar el servicio Orchestrator
// --------------------
builder.Services.AddScoped<IOrchestatorService, OrchestratorService>();

// --------------------
// JWT Authentication
// --------------------
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("Jwt:Key missing");

var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtKey)
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
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

// --------------------
// Build
// --------------------
var app = builder.Build();

// --------------------
// Pipeline
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();