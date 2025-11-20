using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using WebUI.DTO;

namespace WebUI.Pages.Memberships
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _membershipHttp;
        private readonly HttpClient _disciplineHttp;

        public CreateModel(IHttpClientFactory httpFactory)
        {
            _membershipHttp = httpFactory.CreateClient("Memberships");
            _disciplineHttp = httpFactory.CreateClient("Disciplines");
        }

        [BindProperty]
        public MembershipDTO Membership { get; set; } = new();

        // Lista de disciplinas para mostrar en el formulario
        public List<DisciplineDTO> Disciplines { get; set; } = new();

        // Lista de IDs seleccionados
        [BindProperty]
        public List<short> SelectedDisciplineIds { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                var data = await _disciplineHttp.GetFromJsonAsync<List<DisciplineDTO>>("/api/Disciplines");
                Disciplines = data?.OrderBy(d => d.Name).ToList() ?? new List<DisciplineDTO>();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar las disciplinas.";
                Console.WriteLine("Error al obtener disciplinas: " + ex.Message);
                Disciplines = new List<DisciplineDTO>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var membershipToSend = new
                {
                    Name = Membership.Name,
                    Price = Membership.Price,
                    Description = Membership.Description,
                    MonthlySessions = Membership.MonthlySessions,
                    IsActive = Membership.IsActive,
                    DisciplineIds = SelectedDisciplineIds // Enviamos las disciplinas seleccionadas
                };

                var resp = await _membershipHttp.PostAsJsonAsync("/api/Memberships", membershipToSend);

                if (resp.IsSuccessStatusCode)
                {
                    var created = await resp.Content.ReadFromJsonAsync<MembershipDTO>();
                    TempData["SuccessMessage"] = $"Membresía creada exitosamente. Id: {created?.Id ?? 0}";
                    return RedirectToPage("./Index");
                }

                var errorContent = await resp.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = "No se pudo crear la membresía: " + errorContent;
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al conectar con el microservicio: " + ex.Message;
                return Page();
            }
        }
    }
}
