using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;
using WebUI.DTO;

namespace WebUI.Pages.Clients;

public class CreateModel : PageModel
{
    private readonly HttpClient _http;

    [BindProperty]
    public ClientDto Client { get; set; } = new();

    public CreateModel(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ClientAPI");
    }

    public void OnGet() { }

    public IActionResult OnGetPartial()
    {
        Client = new ClientDto();
        return Partial("_CreateClientForm", this);
    }

    public async Task<IActionResult> OnPostPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Devuelve el Partial si hay errores
            return Partial("_CreateClientForm", this);
        }

        var resp = await _http.PostAsJsonAsync("/api/Client", Client);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            var message = ExtractErrorMessage(body, resp.StatusCode, "crear el cliente");
            ModelState.AddModelError(string.Empty, message);
            return Partial("_CreateClientForm", this);
        }

        // 🔹 Leer el ID del cliente recién creado desde la API
        int newId = 0;
        try
        {
            var responseString = await resp.Content.ReadAsStringAsync();

            // Si la API devuelve un entero puro, intenta parsearlo
            if (!int.TryParse(responseString, out newId))
            {
                // Si devuelve un objeto JSON {"id":5,...}
                using var doc = JsonDocument.Parse(responseString);
                if (doc.RootElement.TryGetProperty("id", out var idProp))
                {
                    newId = idProp.GetInt32();
                }
            }
        }
        catch
        {
            newId = 0; // fallback
        }

        // 🔹 Devuelve JSON que espera Sales
        return new JsonResult(new
        {
            success = true,
            newId = newId,
            fullName = $"{Client.Name} {Client.FirstLastname} {Client.SecondLastname}".Trim(),
            ci = Client.Ci
        });
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var resp = await _http.PostAsJsonAsync("/api/Client", Client);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            var message = ExtractErrorMessage(body, resp.StatusCode, "crear el cliente");
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
            catch (JsonException) { }
            return $"Error al {action}: {raw}";
        }
        return $"Error al {action}. Código HTTP: {(int)status} ({status}).";
    }
}