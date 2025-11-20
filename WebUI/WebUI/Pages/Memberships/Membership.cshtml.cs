using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.Memberships
{
    [Authorize(Roles = "Admin,Instructor")]
    public class MembershipModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
