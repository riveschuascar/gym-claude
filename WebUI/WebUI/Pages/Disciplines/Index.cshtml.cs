using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Disciplines;

public class IndexModel : PageModel
{
    private readonly HttpClient _disciplineHttp;

    // Lista de disciplinas que se mostrará en la UI
    public List<DisciplineDTO> Disciplines { get; set; } = new();

    public IndexModel(IHttpClientFactory factory)
    {
        // Asegúrate de que el HttpClient "Disciplines" tenga BaseAddress = http://localhost:5098
        _disciplineHttp = factory.CreateClient("Disciplines");
    }

    public async Task OnGet()
    {
        try
        {
            // URL corregida: minúscula y coincidiendo con tu API
            var data = await _disciplineHttp.GetFromJsonAsync<List<DisciplineDTO>>("/api/disciplines");

            Disciplines = (data ?? new List<DisciplineDTO>())
                .OrderBy(d => d.Name)
                .ThenBy(d => d.StartTime)
                .ToList();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al cargar las disciplinas.";
            Console.WriteLine("Error al obtener disciplinas: " + ex.Message);
            Disciplines = new List<DisciplineDTO>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            var resp = await _disciplineHttp.DeleteAsync($"/api/disciplines/Eliminar/{id}");
            TempData[resp.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                resp.IsSuccessStatusCode ? "Disciplina eliminada exitosamente." : "No se pudo eliminar la disciplina.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al conectar con el microservicio.";
            Console.WriteLine("Error al eliminar disciplina: " + ex.Message);
        }

        return RedirectToPage();
    }
}

