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
            var resp = await _disciplineHttp.PutAsJsonAsync($"/api/Discipline/{Discipline.Id}", Discipline);
            TempData[resp.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                resp.IsSuccessStatusCode ? "Disciplina actualizada exitosamente." : "No se pudo actualizar la disciplina.";
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