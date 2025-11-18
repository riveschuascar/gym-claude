using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Users;

public class IndexModel : PageModel
{
    private readonly HttpClient _userHttp;
    public List<UserDTO> Users { get; set; } = new();

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _userHttp = httpClientFactory.CreateClient("Users");
    }

    public async Task OnGet()
    {
        try
        {
            var data = await _http.GetFromJsonAsync<List<UserDTO>>("/api/User");
            Clients = (data ?? new List<UserDTO>())
                .OrderBy(u => u.Name)
                .ThenBy(u => u.FirstLastname)
                .ThenBy(u => u.SecondLastname)
                .ToList();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "No se pudo conectar con el microservicio de Usuarios. Verifique que esté en ejecución (puerto configurado) y vuelva a intentar.";
            Clients = new List<UserDTO>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var resp = await _http.DeleteAsync($"/api/User/{id}");
        // Ignoramos el estado para simplificar, refrescamos la lista
        return RedirectToPage();
    }
}