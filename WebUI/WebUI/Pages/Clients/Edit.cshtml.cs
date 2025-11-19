using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Common;
using WebUI.DTO;
using WebUI.Validation;

namespace WebUI.Pages.Clients;

public class EditModel : PageModel
{
    private readonly HttpClient _http;

    [BindProperty]
    public ClientDto Client { get; set; } = new();

    public EditModel(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ClientAPI");
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var data = await _http.GetFromJsonAsync<ClientDto>($"/api/Client/{id}");
        if (data is null) return NotFound();
        Client = data;
        return Page();
    }

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

        var resp = await _http.PutAsJsonAsync($"/api/Client/{Client.Id}", Client);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            string message;
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                message = $"No se encontró el cliente con ID {Client.Id} para actualizar.";
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace(body))
            {
                message = $"Error de validación al actualizar: {body}";
            }
            else
            {
                message = string.IsNullOrWhiteSpace(body)
                    ? $"Error al actualizar el cliente. Código HTTP: {resp.StatusCode}."
                    : $"Error al actualizar el cliente: {body}";
            }

            ModelState.AddModelError(string.Empty, message);
            return Page();
        }

        return RedirectToPage("Index");
    }
}