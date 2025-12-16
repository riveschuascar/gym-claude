using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;
using WebUI.DTO;

namespace WebUI.Pages.Disciplines;

public class CreateModel : PageModel
{
    private readonly HttpClient _disciplineHttp;
    [BindProperty]
    public DisciplineDTO Discipline { get; set; } = new();

    public CreateModel(IHttpClientFactory factory)
    {
        _disciplineHttp = factory.CreateClient("Disciplines");
    }

    public void OnGet() { }

    public IActionResult OnGetPartial()
    {
        Discipline = new DisciplineDTO();

        return Partial("_CreateDisciplineForm", this);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var resp = await _disciplineHttp.PostAsJsonAsync("/api/Disciplines", Discipline);
            if (resp.IsSuccessStatusCode)
            {
                return new JsonResult(new { success = true });
            }
            else
            {
                var error = await resp.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = ExtractErrorMessage(error, resp.StatusCode, "crear la disciplina");
                return Page();
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al conectar con el microservicio.";
            Console.WriteLine(ex.Message);
            return Page();
        }
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
                    return err.GetString() ?? $"No se pudo {action}.";
                }
            }
            catch (JsonException)
            {

            }
            return $"No se pudo {action}: {raw}";
        }

        return $"No se pudo {action}. Código HTTP: {(int)status} ({status}).";
    }
}