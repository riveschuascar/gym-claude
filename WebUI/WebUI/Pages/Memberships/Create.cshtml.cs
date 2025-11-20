using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using WebUI.DTO;

namespace WebUI.Pages.Memberships
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _membershipHttp;

        public CreateModel(IHttpClientFactory factory)
        {
            _membershipHttp = factory.CreateClient("Memberships");
        }

        [BindProperty]
        public MembershipDTO Membership { get; set; } = new();

        public short? CreatedId { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var resp = await _membershipHttp.PostAsJsonAsync("/api/Memberships", Membership);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Membresía creada exitosamente. Id: {Membership.Id}";
                    return RedirectToPage("./Index");
                }

                var errorContent = await resp.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "No se pudo crear la membresía: " + errorContent);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al conectar con el microservicio: " + ex.Message);
                return Page();
            }
        }
    }
}
