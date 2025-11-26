using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;
using WebUI.DTO;

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

        var resp = await _http.PutAsJsonAsync($"/api/Client/{Client.Id}", Client);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            string message;
            if (resp.StatusCode == HttpStatusCode.NotFound)
            {
                message = $"No se encontró el cliente con ID {Client.Id} para actualizar.";
            }
            else
            {
                message = ExtractErrorMessage(body, resp.StatusCode, "actualizar el cliente");
            }

            ModelState.AddModelError(string.Empty, message);
            return Page();
        }

        return RedirectToPage("Index");
    }

    private static string ExtractErrorMessage(string raw, HttpStatusCode status, string action)
    {
        if (!string.IsNullOrWhiteSpace(raw))
        {
            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("error", out var err) && err.ValueKind == JsonValueKind.String)
                {
                    return err.GetString() ?? $"Error al {action}.";
                }
            }
            catch (JsonException)
            {
                // ignoramos parseo, usamos texto crudo
            }

            return $"Error al {action}: {raw}";
        }

        return $"Error al {action}. Código HTTP: {(int)status} ({status}).";
    }
}

