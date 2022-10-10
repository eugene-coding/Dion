using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Shared;

namespace Web.Server.Pages;

public class SignoutModel : PageModel
{
    public IActionResult OnGet()
    {
        return SignOut(Config.CookieSchemeName, Config.OidcSchemeName);
    }
}
