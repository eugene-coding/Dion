using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Login.Password;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    public string Username { get; private set; }

    [BindProperty]
    public InputModel Input { get; set; }

    public void OnGet(string username)
    {
        Username = username;

        Input = new();
    }

    public JsonResult OnPostValidateUsername()
    {
        return new JsonResult(true);
    }

    public IActionResult OnGetSuccess(string query)
    {
        return Redirect("/Account/Login/Password" + query);
    }
}
