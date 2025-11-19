using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Disciplines;

public class DeleteModel : PageModel
{
    private readonly HttpClient _disciplineHttp;
    [BindProperty]
    public DisciplineDTO Discipline { get; set; } = new();

    public DeleteModel(IHttpClientFactory factory)
    {
        _disciplineHttp = factory.CreateClient("Disciplines");
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var data = await _disciplineHttp.GetFromJsonAsync<DisciplineDTO>($"/api/Discipline/{id}");
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

    public async Task<IActionResult> OnPostAsync(int id)
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

        return RedirectToPage("Index");
    }
}
