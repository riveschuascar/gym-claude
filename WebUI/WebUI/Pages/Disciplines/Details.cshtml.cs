using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Disciplines;

public class DetailsModel : PageModel
{
    private readonly HttpClient _disciplineHttp;
    public DisciplineDTO Discipline { get; set; } = new();

    public DetailsModel(IHttpClientFactory factory)
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
}