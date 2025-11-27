using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;
using System.Text.Json;

namespace WebUI.Pages.Users;

public class CreateModel : PageModel
{
    private readonly HttpClient _userHttp;

    [BindProperty]
    public UserDTO User { get; set; } = new();

    public CreateModel(IHttpClientFactory httpClientFactory)
    {
        _userHttp = httpClientFactory.CreateClient("Users");
    }

    public void OnGet()
    {
        // Página simplemente carga el formulario vacío
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var userToCreate = new UserDTO
            {
                Name = User.Name,
                FirstLastname = User.FirstLastname,
                SecondLastname = User.SecondLastname,
                DateOfBirth = User.DateOfBirth,
                Ci = User.Ci,
                UserRole = User.UserRole,
                HireDate = User.HireDate,
                MonthlySalary = User.MonthlySalary,
                Specialization = User.Specialization,
                Email = User.Email
            };

            var response = await _userHttp.PostAsJsonAsync("/api/User", userToCreate);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Usuario creado exitosamente.";
                return RedirectToPage("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                string errorMessage = ExtractErrorMessage(errorContent);
                TempData["ErrorMessage"] = $"Error: {errorMessage}";
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

    private string ExtractErrorMessage(string jsonResponse)
    {
        try
        {
            using var jsonDoc = JsonDocument.Parse(jsonResponse);
            var root = jsonDoc.RootElement;

            // Intenta obtener la propiedad "error"
            if (root.TryGetProperty("error", out var errorElement))
            {
                return errorElement.GetString() ?? "Error desconocido";
            }

            // Si no existe "error", retorna el JSON completo
            return jsonResponse;
        }
        catch
        {
            // Si no es un JSON válido, retorna el contenido tal cual
            return jsonResponse;
        }
    }
}