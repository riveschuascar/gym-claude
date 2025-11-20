using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Disciplines;

public class EditModel : PageModel
{
    private readonly HttpClient _disciplineHttp;
    [BindProperty]
    public DisciplineDTO Discipline { get; set; } = new();

    public EditModel(IHttpClientFactory factory)
    {
        _disciplineHttp = factory.CreateClient("Disciplines");
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var data = await _disciplineHttp.GetFromJsonAsync<DisciplineDTO>($"/api/Disciplines/{id}");
            if (data == null) return RedirectToPage("Index");
            Discipline = data;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al cargar la disciplina.";
            Console.WriteLine(ex.Message);
            return RedirectToPage("Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var resp = await _disciplineHttp.PutAsJsonAsync($"/api/Disciplines/{Discipline.Id}", Discipline);
            if (resp.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Disciplina actualizada exitosamente.";
            }
            else
            {
                var error = await resp.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = string.IsNullOrWhiteSpace(error)
                    ? "No se pudo actualizar la disciplina."
                    : $"No se pudo actualizar la disciplina: {error}";
                return Page();
            }
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] = "No se pudo conectar con el microservicio de Disciplinas.";
            return Page();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al conectar con el microservicio.";
            Console.WriteLine(ex.Message);
            return Page();
        }

        return RedirectToPage("Index");
    }
}
