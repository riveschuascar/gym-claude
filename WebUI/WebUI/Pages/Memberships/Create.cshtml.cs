using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using WebUI.DTO;

namespace WebUI.Pages.Memberships
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _membershipHttp;

        public CreateModel(IHttpClientFactory httpFactory)
        {
            _membershipHttp = httpFactory.CreateClient("Memberships");
        }

        [BindProperty]
        public MembershipDTO Membership { get; set; } = new MembershipDTO();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Preparar objeto para enviar sin Id
                var membershipToSend = new
                {
                    Name = Membership.Name,
                    Price = Membership.Price,
                    Description = Membership.Description,
                    MonthlySessions = Membership.MonthlySessions,
                    IsActive = Membership.IsActive
                };

                var resp = await _membershipHttp.PostAsJsonAsync("/api/Memberships", membershipToSend);

                if (resp.IsSuccessStatusCode)
                {
                    // Leer respuesta del microservicio
                    var created = await resp.Content.ReadFromJsonAsync<MembershipDTO>();

                    // Mostrar mensaje de �xito, Id puede ser null mientras el microservicio lo genere
                    TempData["SuccessMessage"] = $"Membresía creada exitosamente.";

                    return RedirectToPage("./Index");
                }

                // Leer error del microservicio
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