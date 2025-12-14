using Microsoft.AspNetCore.Authentication.Cookies;
using WebUI.Common;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TokenMessageHandler>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Login/Index";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdminOrInstructor", policy => policy.RequireRole("Admin", "Instructor"));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Clients", "AdminOrInstructor");
    options.Conventions.AuthorizeFolder("/Memberships", "AdminOrInstructor");
    options.Conventions.AuthorizeFolder("/Sales", "AdminOrInstructor");
    options.Conventions.AuthorizeFolder("/Disciplines", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Users", "AdminOnly");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Privacy");
    options.Conventions.AllowAnonymousToPage("/Login/Index");
    options.Conventions.AllowAnonymousToPage("/Login/Logout");
    options.Conventions.AllowAnonymousToPage("/Error");
});


builder.Services.AddHttpClient("Memberships", client =>
{
    var baseUrl = builder.Configuration["MembershipApiBase"] ?? "http://localhost:5292";
    client.BaseAddress = new Uri(baseUrl);
}).AddHttpMessageHandler<TokenMessageHandler>();

// Registramos Microservicios
builder.Services.AddHttpClient("Disciplines", client =>
{
    var baseUrl = builder.Configuration["DisciplineApiBase"] ?? "http://localhost:5098";
    client.BaseAddress = new Uri(baseUrl);
}).AddHttpMessageHandler<TokenMessageHandler>();

builder.Services.AddHttpClient("Users", client =>
{
    var baseUrl = builder.Configuration["UserApiBase"] ?? "http://localhost:5089";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<TokenMessageHandler>();

builder.Services.AddHttpClient("ClientAPI", client =>
{
    var baseUrl = builder.Configuration["ClientApiBase"] ?? "http://localhost:5135";
    client.BaseAddress = new Uri(baseUrl);
}).AddHttpMessageHandler<TokenMessageHandler>();

builder.Services.AddHttpClient("LoginAPI", client =>
{
    var baseUrl = builder.Configuration["LoginApiBase"] ?? "http://localhost:5289";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("SalesAPI", client =>
{
    var baseUrl = builder.Configuration["SalesApiBase"] ?? "http://localhost:5305";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<TokenMessageHandler>();

builder.Services.AddHttpClient("ReportAPI", client =>
{
    var baseUrl = builder.Configuration["ReportApiBase"] ?? "http://localhost:5236";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<TokenMessageHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
