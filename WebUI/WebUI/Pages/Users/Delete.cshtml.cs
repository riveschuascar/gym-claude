using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Users;

public class DeleteModel : PageModel
{
    private readonly HttpClient _userHttp;

    [BindProperty]
    public UserDTO User { get; set; } = new();

    public DeleteModel(IHttpClientFactory httpClientFactory)
    {
        _userHttp = httpClientFactory.CreateClient("Users");
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _userHttp.GetFromJsonAsync<UserDTO>($"/api/User/{id}");
        if (user is null) return NotFound();
        User = user;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var resp = await _userHttp.DeleteAsync($"/api/User/{id}");
        if (!resp.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, $"Error al eliminar: {resp.StatusCode}");
            return Page();
        }
        return RedirectToPage("Index");
    }
}
