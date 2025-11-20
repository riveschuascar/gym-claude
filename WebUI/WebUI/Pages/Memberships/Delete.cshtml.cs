using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.Memberships
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _membershipHttp;

        public DeleteModel(IHttpClientFactory factory)
        {
            _membershipHttp = factory.CreateClient("Memberships");
        }

        [BindProperty]
        public short Id { get; set; }

        public async Task<IActionResult> OnGetAsync(short id)
        {
            Id = id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"Intentando eliminar ID: {Id}");

            var resp = await _membershipHttp.DeleteAsync($"/api/Memberships/{Id}");

            Console.WriteLine("Status DELETE:");
            Console.WriteLine(resp.StatusCode);

            if (resp.IsSuccessStatusCode)
                return RedirectToPage("./Index");

            ModelState.AddModelError(string.Empty, "Error eliminando la membresía.");
            return Page();
        }
    }
}