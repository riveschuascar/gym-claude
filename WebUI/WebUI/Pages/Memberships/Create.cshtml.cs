using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Memberships;

public class CreateModel : PageModel
{
    private readonly HttpClient _membershipHttp;

    [BindProperty]
    public MembershipDTO Membership { get; set; } = new();

    public CreateModel(IHttpClientFactory factory)
    {
        _membershipHttp = factory.CreateClient("Memberships");
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var resp = await _membershipHttp.PostAsJsonAsync("/api/Memberships", Membership);
            if (resp.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Membresía creada exitosamente.";
            else
                TempData["ErrorMessage"] = "No se pudo crear la membresía.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error al conectar con el microservicio.";
            Console.WriteLine(ex.Message);
            return Page();
        }

        return RedirectToPage("Index");
    }
}