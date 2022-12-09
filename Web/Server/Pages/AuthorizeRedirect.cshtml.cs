using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Server.Pages;

public class AuthorizeRedirectModel : PageModel
{
    public LocalRedirectResult OnGet()
    {
        return LocalRedirect("~/");
    }
}
