using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Memberships
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _membershipHttp;

        public List<MembershipDTO> Memberships { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _membershipHttp = httpClientFactory.CreateClient("Memberships");
        }

        public async Task OnGetAsync()
        {
            try
            {
                var data = await _membershipHttp.GetFromJsonAsync<List<MembershipDTO>>("/api/Memberships");

                Memberships = (data ?? new List<MembershipDTO>())
                    .OrderBy(m => m.Name)         
                    .ThenBy(m => m.Price ?? 0)    
                    .ToList();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar las membresías.";
                Console.WriteLine("Error al obtener membresías: " + ex.Message);
                Memberships = new List<MembershipDTO>();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(short id)
        {
            try
            {
                var resp = await _membershipHttp.DeleteAsync($"/api/Memberships/{id}");
                TempData[resp.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                    resp.IsSuccessStatusCode ? "Membresía eliminada exitosamente." : "No se pudo eliminar la membresía.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al conectar con el microservicio.";
                Console.WriteLine("Error al eliminar membresía: " + ex.Message);
            }

            return RedirectToPage();
        }
    }
}
