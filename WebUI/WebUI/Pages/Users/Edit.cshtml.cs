using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Users;

public class EditModel : PageModel
{
    private readonly HttpClient _userHttp;
    private readonly ILogger<EditModel> _logger;

    [BindProperty]
    public UserDTO User { get; set; } = new();

    public EditModel(IHttpClientFactory httpClientFactory, ILogger<EditModel> logger)
    {
        _userHttp = httpClientFactory.CreateClient("Users");
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] = "ID de usuario inválido.";
            return RedirectToPage("Index");
        }

        try
        {
            _logger.LogInformation($"Intentando obtener usuario con ID: {id}");
            
            var user = await _userHttp.GetFromJsonAsync<UserDTO>($"/api/User/id/{id}");

            if (user == null)
            {
                _logger.LogWarning($"Usuario con ID {id} no encontrado (respuesta nula)");
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToPage("Index");
            }

            User = user;
            _logger.LogInformation($"Usuario con ID {id} cargado exitosamente");
            return Page();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning($"Usuario con ID {id} no encontrado (404)");
            TempData["ErrorMessage"] = "Usuario no encontrado.";
            return RedirectToPage("Index");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Error HTTP al obtener usuario {id}: {ex.StatusCode} - {ex.Message}");
            TempData["ErrorMessage"] = $"Error de conexión: {ex.StatusCode}. Intente nuevamente.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error inesperado al cargar usuario con ID {id}");
            TempData["ErrorMessage"] = "No se pudo cargar el usuario. Intente nuevamente.";
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            _logger.LogWarning($"ModelState inválido: {string.Join(", ", errors.Select(e => e.ErrorMessage))}");
            return Page();
        }

        try
        {
            _logger.LogInformation($"Intentando actualizar usuario con ID: {User.Id}");
            
            var response = await _userHttp.PutAsJsonAsync("/api/User", User);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Usuario {User.Id} actualizado exitosamente");
                TempData["SuccessMessage"] = "Usuario actualizado exitosamente.";
                return RedirectToPage("Index");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Validación fallida: {content}");
                TempData["ErrorMessage"] = "Datos inválidos. Verifique los campos.";
                return Page();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Usuario {User.Id} no existe al actualizar");
                TempData["ErrorMessage"] = "El usuario no existe.";
                return RedirectToPage("Index");
            }
            else
            {
                _logger.LogError($"Error al actualizar usuario: {response.StatusCode}");
                TempData["ErrorMessage"] = "Error al actualizar el usuario.";
                return Page();
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de conexión al actualizar usuario");
            TempData["ErrorMessage"] = "No se pudo conectar con el microservicio. Intente nuevamente.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar usuario");
            TempData["ErrorMessage"] = "No se pudo conectar con el microservicio. Intente nuevamente.";
            return Page();
        }
    }
}