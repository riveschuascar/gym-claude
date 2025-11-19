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
                Memberships = await _membershipHttp
                    .GetFromJsonAsync<List<MembershipDTO>>("/api/Membership") ?? new();
            }
            catch
            {
                Memberships = new();
            }
        }
    }
}
