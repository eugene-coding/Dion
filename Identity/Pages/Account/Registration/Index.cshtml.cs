using Identity.Exceptions;
using Identity.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Identity.Pages.Account.Registration;

[AllowAnonymous]
[SecurityHeaders]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager, IStringLocalizer<IndexModel> localizer)
    {
        _userManager = userManager;
        Localizer = localizer;
    }

    public IStringLocalizer<IndexModel> Localizer { get; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task OnPostAsync()
    {
        var user = new ApplicationUser()
        {
            UserName = Input.Username,
            Email = Input.Email
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (!result.Succeeded)
        {
            throw new CreateUserFailedException(result.Errors.First().Description);
        }
    }
}
