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
            // Deserializar directamente a List<UserDTO>
            var data = await _userHttp.GetFromJsonAsync<List<UserDTO>>("/api/User");

            Users = (data ?? new List<UserDTO>())
                .OrderBy(u => u.Name)
                .ThenBy(u => u.FirstLastname)
                .ThenBy(u => u.SecondLastname)
                .ToList();
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
        try
        {
            var resp = await _userHttp.DeleteAsync($"/api/User/Eliminar/{id}");

            if (resp.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Usuario eliminado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo eliminar el usuario.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al conectar con el microservicio.";
            Console.WriteLine($"Error: {ex.Message}");
        }

        return RedirectToPage();
    }
}