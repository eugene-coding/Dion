using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Shared;

namespace Identity.Pages.Login.Password;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; private init; } = new();

    public string Username { get; private set; }

    private ISession Session => HttpContext.Session;

    public IActionResult OnGet()
    {
        var username = Session.GetString(SessionKeys.Username);

        if (!string.IsNullOrEmpty(username))
        {
            Username = username;
        }
        else
        {
            return LocalRedirect("~/Account/Login" + Request.QueryString);
        }

        return Page();
    }
}
