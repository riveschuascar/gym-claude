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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("ENTRÓ A OnPostAsync");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState inválido");
                return Page();
            }

            try
            {
                Console.WriteLine("Enviando POST al microservicio...");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(Membership));

                var resp = await _membershipHttp.PostAsJsonAsync("/api/Memberships", Membership);
                Console.WriteLine("StatusCode: " + resp.StatusCode);

                if (resp.IsSuccessStatusCode)
                    TempData["SuccessMessage"] = "Membresía creada exitosamente.";
                else
                    TempData["ErrorMessage"] = "No se pudo crear la membresía.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al conectar con el microservicio.";
                Console.WriteLine("EXCEPCIÓN: " + ex.Message);
                return Page();
            }

            return RedirectToPage("Index");
        }

    }
}
