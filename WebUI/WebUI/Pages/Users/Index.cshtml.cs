using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Common;
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
            // Deserializar directamente a Result<List<UserDTO>>
            var response = await _userHttp.GetFromJsonAsync<Result<List<UserDTO>>>("/api/User");

            if (response?.Value != null)
            {
                Users = response.Value
                    .OrderBy(u => u.Name)
                    .ThenBy(u => u.FirstLastname)
                    .ThenBy(u => u.SecondLastname)
                    .ToList();
            }
            else
            {
                Users = new List<UserDTO>();
            }
        }
        catch (HttpRequestException ex)
        {
            TempData["ErrorMessage"] = "No se pudo conectar con el microservicio de Usuarios. Verifique que esté en ejecución (puerto 5089).";
            Users = new List<UserDTO>();
            Console.WriteLine($"Error de conexión: {ex.Message}");
        }
        catch (System.Text.Json.JsonException ex)
        {
            TempData["ErrorMessage"] = "Error al procesar los datos del microservicio.";
            Users = new List<UserDTO>();
            Console.WriteLine($"Error de JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Ocurrió un error inesperado al cargar los usuarios.";
            Users = new List<UserDTO>();
            Console.WriteLine($"Error general: {ex.Message}");
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var resp = await _userHttp.DeleteAsync($"/api/User/{id}");
        // Ignoramos el estado para simplificar, refrescamos la lista
        return RedirectToPage();
    }
}