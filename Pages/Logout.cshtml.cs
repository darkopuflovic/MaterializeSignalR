using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace MaterializeSignalR.Models
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.SignOutAsync();

            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Avatar");
            HttpContext.Session.Remove("Color");
            HttpContext.Session.Remove("Name");

            return Redirect("~/");
        }
    }
}
