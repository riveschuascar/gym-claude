using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Memberships
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _membershipClient;
        private readonly HttpClient _disciplineClient;

        public DetailsModel(IHttpClientFactory factory)
        {
            _membershipClient = factory.CreateClient("Memberships");
            _disciplineClient = factory.CreateClient("Disciplines");
        }

        [BindProperty(SupportsGet = true)]
        public short Id { get; set; }

        public MembershipDTO? Membership { get; set; }
        public List<DisciplineDTO> Disciplines { get; set; } = new();
        public List<DisciplineDTO> AssignedDisciplines { get; set; } = new();

        [BindProperty]
        public int SelectedDisciplineId { get; set; }

        public IEnumerable<DisciplineDTO> AvailableDisciplines =>
            Disciplines
                .Where(d => d.IsActive && AssignedDisciplines.All(a => a.Id != d.Id))
                .OrderBy(d => d.Name);

        private async Task LoadDataAsync()
        {
            Membership = await _membershipClient.GetFromJsonAsync<MembershipDTO>($"/api/Memberships/{Id}");

            var detail = await _membershipClient.GetFromJsonAsync<List<MembershipDisciplineDTO>>($"/api/MembershipDetails/{Id}")
                         ?? new List<MembershipDisciplineDTO>();

            Disciplines = await _disciplineClient.GetFromJsonAsync<List<DisciplineDTO>>("/api/Disciplines")
                          ?? new List<DisciplineDTO>();

            var assignedIds = detail.Where(d => d.IsActive).Select(d => d.DisciplineId).ToHashSet();
            AssignedDisciplines = Disciplines
                .Where(d => assignedIds.Contains(d.Id))
                .OrderBy(d => d.Name)
                .ToList();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await LoadDataAsync();

                if (Membership == null)
                {
                    TempData["ErrorMessage"] = "No se encontro la membresia solicitada.";
                    return RedirectToPage("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "No se pudo cargar el detalle de la membresia.";
                Console.WriteLine("Error al obtener detalle de membresia: " + ex.Message);
                return RedirectToPage("Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            try
            {
                if (SelectedDisciplineId <= 0)
                {
                    TempData["ErrorMessage"] = "Seleccione una disciplina valida.";
                    return RedirectToPage(new { id = Id });
                }

                var resp = await _membershipClient.PostAsJsonAsync("/api/MembershipDetails", new
                {
                    membershipId = Id,
                    disciplineId = SelectedDisciplineId
                });

                TempData[resp.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                    resp.IsSuccessStatusCode
                        ? "Disciplina agregada a la membresia."
                        : "No se pudo agregar la disciplina. Verifique que no este ya asignada.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al conectar con el microservicio.";
                Console.WriteLine("Error al agregar disciplina: " + ex.Message);
            }

            return RedirectToPage(new { id = Id });
        }

        public async Task<IActionResult> OnPostRemoveAsync(int disciplineId)
        {
            try
            {
                var resp = await _membershipClient.DeleteAsync($"/api/MembershipDetails/{Id}/{disciplineId}");
                TempData[resp.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                    resp.IsSuccessStatusCode
                        ? "Disciplina removida de la membresia."
                        : "No se pudo remover la disciplina.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al conectar con el microservicio.";
                Console.WriteLine("Error al remover disciplina: " + ex.Message);
            }

            return RedirectToPage(new { id = Id });
        }
    }
}
