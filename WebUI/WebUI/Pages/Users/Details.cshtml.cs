using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Users;

public class DetailsModel : PageModel
{
    private readonly HttpClient _userHttp;
    public UserDTO User { get; set; } = new();

    public DetailsModel(IHttpClientFactory factory)
    {
        _userHttp = factory.CreateClient("Users");
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var data = await _userHttp.GetFromJsonAsync<UserDTO>($"/api/User/id/{id}");
            if (data is null) return NotFound();
            User = data;
            return Page();
        }
        catch
        {
            return NotFound();
        }
    }
}
