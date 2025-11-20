using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using WebUI.DTO;

namespace WebUI.Pages.Memberships;

public class EditModel : PageModel
{
    private readonly HttpClient _membershipHttp;
    private readonly HttpClient _disciplineHttp;

    public EditModel(IHttpClientFactory factory)
    {
        _membershipHttp = factory.CreateClient("Memberships");
        _disciplineHttp = factory.CreateClient("Disciplines");
    }

    [BindProperty]
    public MembershipDTO Membership { get; set; } = new();

    public List<DisciplineDTO> Disciplines { get; set; } = new();

    [BindProperty]
    public List<short> SelectedDisciplineIds { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var data = await _membershipHttp.GetFromJsonAsync<MembershipDTO>($"/api/Memberships/{id}");
            if (data == null) return RedirectToPage("Index");

            Membership = data;
            SelectedDisciplineIds = data.DisciplineIds ?? new List<short>();

            var disciplinesData = await _disciplineHttp.GetFromJsonAsync<List<DisciplineDTO>>("/api/Disciplines");
            Disciplines = disciplinesData?.OrderBy(d => d.Name).ToList() ?? new List<DisciplineDTO>();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al cargar la membresía o disciplinas.";
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
            var membershipToSend = new
            {
                Id = Membership.Id,
                Name = Membership.Name,
                Price = Membership.Price,
                Description = Membership.Description,
                MonthlySessions = Membership.MonthlySessions,
                IsActive = Membership.IsActive,
                DisciplineIds = SelectedDisciplineIds
            };

            var resp = await _membershipHttp.PutAsJsonAsync($"/api/Memberships/{Membership.Id}", membershipToSend);

            TempData[resp.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                resp.IsSuccessStatusCode ? "Membresía actualizada exitosamente." : "No se pudo actualizar la membresía.";
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