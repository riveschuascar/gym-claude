using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Clients;

public class DetailsModel : PageModel
{
    private readonly HttpClient _http;
    public ClientDto Client { get; set; } = new();

    public DetailsModel(IHttpClientFactory factory)
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
}