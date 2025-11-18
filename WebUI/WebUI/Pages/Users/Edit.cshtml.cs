using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Users;

public class EditModel : PageModel
{
    private readonly HttpClient _userHttp;

    [BindProperty]
    public UserDTO User { get; set; } = new();

    public EditModel(IHttpClientFactory httpClientFactory)
    {
        _userHttp = httpClientFactory.CreateClient("Users");
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var user = await _userHttp.GetFromJsonAsync<UserDTO>($"/api/User/{id}");

            if (user == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToPage("Index");
            }

            User = user;
            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "No se pudo cargar el usuario. Intente nuevamente.";
            Console.WriteLine($"Error: {ex.Message}");
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var response = await _userHttp.PutAsJsonAsync("/api/User/Modificar", User);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Usuario actualizado exitosamente.";
                return RedirectToPage("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Error al actualizar el usuario.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "No se pudo conectar con el microservicio. Intente nuevamente.";
            Console.WriteLine($"Error: {ex.Message}");
            return Page();
        }
    }
}