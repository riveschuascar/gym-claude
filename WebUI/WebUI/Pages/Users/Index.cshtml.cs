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

    public async Task OnGetAsync()
    {
        try
        {
            var data = await _userHttp.GetFromJsonAsync<List<UserDTO>>("/api/user");

            Users = data ?? [];               
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] = "No se pudo conectar con el microservicio de Usuarios. Verifique que esté en ejecución (puerto 5089).";
            Users = new List<UserDTO>();
        }
        catch (System.Text.Json.JsonException)
        {
            TempData["ErrorMessage"] = "Error al procesar los datos del microservicio.";
            Users = new List<UserDTO>();
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
            var response = await _userHttp.DeleteAsync($"/api/user/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Usuario eliminado exitosamente.";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "El usuario no existe.";
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