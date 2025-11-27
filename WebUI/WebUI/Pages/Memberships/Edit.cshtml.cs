using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Memberships;

public class EditModel : PageModel
{
    private readonly HttpClient _membershipHttp;

    [BindProperty]
    public MembershipDTO Membership { get; set; } = new();

    public EditModel(IHttpClientFactory factory)
    {
        _membershipHttp = factory.CreateClient("Memberships");
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var data = await _membershipHttp.GetFromJsonAsync<MembershipDTO>($"/api/Memberships/{id}");
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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Aquí agregamos el debug
            Console.WriteLine("Datos que se enviarón a la API:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(Membership));

            var resp = await _membershipHttp.PutAsJsonAsync($"/api/Memberships/{Membership.Id}", Membership);

            Console.WriteLine("Status Code de la API: " + resp.StatusCode);

            TempData[resp.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
                resp.IsSuccessStatusCode ? "Membresía actualizada exitosamente." : "No se pudo actualizar la membresía.";
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