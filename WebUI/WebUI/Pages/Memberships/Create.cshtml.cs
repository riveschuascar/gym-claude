using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Memberships
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _membershipHttp;

        [BindProperty]
        public MembershipDTO Membership { get; set; } = new();

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _membershipHttp = httpClientFactory.CreateClient("Memberships");
        }

        public void OnGet()
        {
            // Página simplemente carga el formulario vacío
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var membershipToCreate = new MembershipDTO
                {
                    Name = Membership.Name,
                    Price = Membership.Price,
                    Description = Membership.Description,
                    MonthlySessions = Membership.MonthlySessions
                };

                var response = await _membershipHttp.PostAsJsonAsync("/api/Membership", membershipToCreate);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Membresía creada exitosamente.";
                    return RedirectToPage("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Error al crear la membresía: {response.StatusCode}";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "No se pudo conectar con el microservicio. Intente nuevamente.";
                Console.WriteLine($"Error: {ex.Message}");
                return Page();
            }
        }
    }
}