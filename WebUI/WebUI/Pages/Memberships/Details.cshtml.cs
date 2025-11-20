using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using WebUI.DTO;

namespace WebUI.Pages.Memberships;

public class DetailsModel : PageModel
{
    private readonly HttpClient _membershipHttp;
    private readonly HttpClient _disciplineHttp;

    public DetailsModel(IHttpClientFactory factory)
    {
        _membershipHttp = factory.CreateClient("Memberships");
        _disciplineHttp = factory.CreateClient("Disciplines");
    }

    [BindProperty]
    public MembershipDTO Membership { get; set; } = new();

    public List<DisciplineDTO> MembershipDisciplines { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            // Obtener membresía
            var membershipData = await _membershipHttp.GetFromJsonAsync<MembershipDTO>($"/api/Memberships/{id}");
            if (membershipData == null) return RedirectToPage("Index");

            Membership = membershipData;

            // Obtener todas las disciplinas
            var allDisciplines = await _disciplineHttp.GetFromJsonAsync<List<DisciplineDTO>>("/api/Disciplines");
            if (allDisciplines != null && Membership.DisciplineIds != null)
            {
                MembershipDisciplines = allDisciplines
                    .Where(d => Membership.DisciplineIds.Contains(d.Id))
                    .OrderBy(d => d.Name)
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al cargar la membresía o disciplinas.";
            Console.WriteLine(ex.Message);
            return RedirectToPage("Index");
        }

        return Page();
    }
}
