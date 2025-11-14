using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Clients;

public class IndexModel : PageModel
{
    private readonly HttpClient _http;
    public List<ClientDto> Clients { get; set; } = new();

    public IndexModel(IConfiguration configuration)
    {
        var baseUrl = configuration["ClientApiBase"] ?? "http://localhost:5135";
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task OnGet()
    {
        try
        {
            var data = await _http.GetFromJsonAsync<List<ClientDto>>("/api/Client");
            Clients = (data ?? new List<ClientDto>())
                .OrderBy(c => c.Name)
                .ThenBy(c => c.FirstLastname)
                .ThenBy(c => c.SecondLastname)
                .ToList();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "No se pudo conectar con el microservicio de Clientes. Verifique que esté en ejecución (puerto configurado) y vuelva a intentar.";
            Clients = new List<ClientDto>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var resp = await _http.DeleteAsync($"/api/Client/{id}");
        // Ignoramos el estado para simplificar, refrescamos la lista
        return RedirectToPage();
    }
}
