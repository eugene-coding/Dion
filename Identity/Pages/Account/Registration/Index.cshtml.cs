using Identity.Exceptions;
using Identity.Models;

using Microsoft.AspNetCore.Identity;

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
            Email = Input.Email
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (!result.Succeeded)
        {
            throw new CreateUserFailedException(result.Errors.First().Description);
        }
    }

    /// <summary>Checks if there is no user with the <see cref="InputModel.Email">email</see> entered.</summary>
    /// <remarks>
    /// If a user with the specified <see cref="InputModel.Email">email</see> is found
    /// and the <see cref="InputModel.Email">email</see> is not confirmed, 
    /// the user will be redirected to the email confirmation page.
    /// </remarks>
    /// <returns>
    /// Returns the <see cref="Task"/> containing the <see cref="JsonResult"/> 
    /// with <see langword="true"/> if a user with the entered email is not found, 
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public async Task<IActionResult> OnPostCheckEmailAsync()
    {
        var user = await _userManager.FindByEmailAsync(Input.Email);

        if (user is not null && !user.EmailConfirmed)
        {
            // Redirect to the email confirmation page
            return LocalRedirect("~/");
        }

        return new JsonResult(user is null);
    }
}
