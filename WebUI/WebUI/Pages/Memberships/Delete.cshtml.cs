using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Memberships
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _membershipHttp;

        [BindProperty]
        public MembershipDTO Membership { get; set; } = new();

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _membershipHttp = httpClientFactory.CreateClient("Memberships");
        }

        public async Task<IActionResult> OnGetAsync(short id)
        {
            var membership = await _membershipHttp.GetFromJsonAsync<MembershipDTO>($"/api/Membership/{id}");

            if (membership == null)
                return RedirectToPage("Index");

            Membership = membership;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(short id)
        {
            var response = await _membershipHttp.DeleteAsync($"/api/Membership/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Membresía eliminada correctamente.";
                return RedirectToPage("Index");
            }

            TempData["ErrorMessage"] = $"Error al eliminar la membresía: {response.StatusCode}";
            return Page();
        }
    }
}
