using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Disciplines;

public class IndexModel : PageModel
{
    private readonly HttpClient _disciplineHttp;
    public List<DisciplineDTO> Disciplines { get; set; } = new();

    public IndexModel(IHttpClientFactory factory)
    {
        _disciplineHttp = factory.CreateClient("Disciplines");
    }

    public async Task OnGet()
    {
        try
        {
            var data = await _disciplineHttp.GetFromJsonAsync<List<DisciplineDTO>>("/api/Discipline");
            Disciplines = (data ?? new List<DisciplineDTO>())
                .OrderBy(d => d.Name)
                .ThenBy(d => d.StartTime)
                .ToList();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al cargar las disciplinas.";
            Console.WriteLine(ex.Message);
            Disciplines = new List<DisciplineDTO>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            var resp = await _disciplineHttp.DeleteAsync($"/api/Discipline/Eliminar/{id}");
            TempData[resp.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                resp.IsSuccessStatusCode ? "Disciplina eliminada exitosamente." : "No se pudo eliminar la disciplina.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al conectar con el microservicio.";
            Console.WriteLine(ex.Message);
        }

        return RedirectToPage();
    }
}
