using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var resp = await _disciplineHttp.PostAsJsonAsync("/api/Disciplines", Discipline);
            if (resp.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Disciplina creada exitosamente.";
            else
                TempData["ErrorMessage"] = "No se pudo crear la disciplina.";
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