using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Clients;

public class DeleteModel : PageModel
{
    private readonly HttpClient _http;

    [BindProperty]
    public ClientDto Client { get; set; } = new();

    public DeleteModel(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("ClientAPI");
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var data = await _http.GetFromJsonAsync<ClientDto>($"/api/Client/{id}");
        if (data is null) return NotFound();
        Client = data;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var resp = await _http.DeleteAsync($"/api/Client/{id}");
        if (!resp.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, $"Error al eliminar: {resp.StatusCode}");
            return Page();
        }
        return RedirectToPage("Index");
    }
}