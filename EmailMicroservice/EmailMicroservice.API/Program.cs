using EmailMicroservice.API.Services;
using EmailMicroservice.Infrastructure.Services;
using EmailMicroservice.Domain.Ports;
using EmailMicroservice.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Register application services and SMTP sender implementation.
// Reads settings from configuration section "Smtp" (see notes below).
builder.Services.AddTransient<EmailSenderService>();
builder.Services.AddSingleton<IEmailSender>(sp =>
{
	var cfg = sp.GetRequiredService<IConfiguration>().GetSection("Smtp");
	var host = cfg.GetValue<string>("Host") ?? string.Empty;
	var port = cfg.GetValue<int?>("Port") ?? 587;
	var username = cfg.GetValue<string>("Username") ?? string.Empty;
	var password = cfg.GetValue<string>("Password") ?? string.Empty;

	return new SmtpEmailSender(host, port, username, password);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<EmailGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
