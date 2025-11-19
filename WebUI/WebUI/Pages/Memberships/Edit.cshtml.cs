using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Memberships
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _membershipHttp;

        [BindProperty]
        public MembershipDTO Membership { get; set; } = new();

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _membershipHttp = httpClientFactory.CreateClient("Memberships");
        }

        public async Task<IActionResult> OnGetAsync(short id)
        {
            try
            {
                var membership = await _membershipHttp.GetFromJsonAsync<MembershipDTO>($"/api/Membership/{id}");

                if (membership == null)
                    return RedirectToPage("Index");

                Membership = membership;
                return Page();
            }
            catch
            {
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var response = await _membershipHttp.PutAsJsonAsync(
                    $"/api/Membership/{Membership.Id}",
                    Membership
                );

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Membresía actualizada correctamente.";
                    return RedirectToPage("Index");
                }

                TempData["ErrorMessage"] = $"Error al actualizar la membresía: {response.StatusCode}";
                return Page();
            }
            catch
            {
                TempData["ErrorMessage"] = "No se pudo conectar con el microservicio.";
                return Page();
            }
        }
    }
}
