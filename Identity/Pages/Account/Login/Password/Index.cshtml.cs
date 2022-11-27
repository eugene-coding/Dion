using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

    public async Task<JsonResult> OnPostValidateUsernameAsync()
    {
        var trimmedUsername = Input.Username.Trim();

        var user = await _userManager.FindByNameAsync(trimmedUsername);
        var valid = user is not null;

        return new JsonResult(valid);
    }

    public IActionResult OnGetSuccess(string query)
    {
        return Redirect("/Account/Login/Password" + query);
    }
}
