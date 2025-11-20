var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddHttpClient("Memberships", client =>
{
    var baseUrl = builder.Configuration["MembershipApiBase"] ?? "http://localhost:5292";
    client.BaseAddress = new Uri(baseUrl);
});

// Registramos Microservicios
builder.Services.AddHttpClient("Disciplines", client =>
{
    var baseUrl = builder.Configuration["DisciplineApiBase"] ?? "http://localhost:5098";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient("Users", client =>
{
    client.BaseAddress = new Uri("http://localhost:5089");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("ClientAPI", client =>
{
    var baseUrl = builder.Configuration["ClientApiBase"] ?? "http://localhost:5135";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();