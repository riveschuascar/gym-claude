var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Agregar HttpClientFactory para llamar a tu microservicio
builder.Services.AddHttpClient("Users", client =>
{
    client.BaseAddress = new Uri("http://localhost:5089");
    client.DefaultRequestHeaders.Add("Accept", "application/json");

builder.Services.AddHttpClient("ClientAPI", client =>
{
    // Usamos tu configuración original, o el puerto por defecto 5135
    var baseUrl = builder.Configuration["ClientApiBase"] ?? "http://localhost:5135";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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