using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Login.Password;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; }

    public void OnGet()
    {
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
