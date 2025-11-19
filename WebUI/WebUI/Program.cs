var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Registrar HttpClient para Disciplinas
builder.Services.AddHttpClient("Disciplines", client =>
{
    // Cambia la URL según tu backend
    var baseUrl = builder.Configuration["DisciplineApiBase"] ?? "http://localhost:5089";
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
