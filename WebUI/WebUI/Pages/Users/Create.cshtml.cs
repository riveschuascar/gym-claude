using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

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
                TempData["ErrorMessage"] = $"Error: {errorContent}";
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