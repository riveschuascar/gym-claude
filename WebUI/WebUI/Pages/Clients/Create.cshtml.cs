using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Common;
using WebUI.DTO;
using WebUI.Validation;

namespace WebUI.Pages.Clients;

public class CreateModel : PageModel
{
    private readonly HttpClient _http;
    [BindProperty]
    public ClientDto Client { get; set; } = new();

    public CreateModel(IConfiguration configuration)
    {
        var baseUrl = configuration["ClientApiBase"] ?? "http://localhost:5135";
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        Result<ClientDto> validation = ClientValidationRules.Validate(Client);
        if (validation.IsFailure)
        {
            ModelState.AddModelError(string.Empty, validation.Error);
            return Page();
        }

        var resp = await _http.PostAsJsonAsync("/api/Client", Client);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            var message = string.IsNullOrWhiteSpace(body)
                ? $"Error al crear el cliente. Código HTTP: {resp.StatusCode}."
                : $"Error al crear el cliente: {body}";
            ModelState.AddModelError(string.Empty, message);
            return Page();
        }
        return RedirectToPage("Index");
    }
}
