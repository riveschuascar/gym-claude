using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Memberships;

public class DeleteModel : PageModel
{
    private readonly HttpClient _membershipHttp;

    [BindProperty]
    public MembershipDTO Membership { get; set; } = new();

    public DeleteModel(IHttpClientFactory factory)
    {
        _membershipHttp = factory.CreateClient("Memberships");
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var data = await _membershipHttp.GetFromJsonAsync<MembershipDTO>($"/api/Membership/{id}");
            if (data == null) return RedirectToPage("Index");
            Membership = data;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al cargar la membresía.";
            Console.WriteLine(ex.Message);
            return RedirectToPage("Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
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
            Console.WriteLine(ex.Message);
        }

        return RedirectToPage("Index");
    }
}
