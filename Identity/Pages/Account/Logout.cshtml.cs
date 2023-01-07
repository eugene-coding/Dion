using Common;

using Identity.Extensions;
using Identity.Models;

using IdentityModel;

using Microsoft.AspNetCore.Identity;

namespace Identity.Pages;

/// <summary>The <see cref="PageModel"/> for the Logout page.</summary>
[SecurityHeaders]
[AllowAnonymous]
public class LogoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;

    private string _logoutId;
    private string _identityProvider;

    /// <summary>Creates the <see cref="LogoutModel"/> instance.</summary>
    /// <param name="signInManager">The <see cref="SignInManager{TUser}"/>.</param>
    /// <param name="interaction">The <see cref="IIdentityServerInteractionService"/>.</param>
    public LogoutModel(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction)
    {
        _signInManager = signInManager;
        _interaction = interaction;
    }

    public async Task<IActionResult> OnGet(string logoutId)
    {
        _logoutId = logoutId;

        if (User?.Identity.IsAuthenticated == true)
        {
            // if there is no logout ID, we need to create one to get info from the current logged user
            _logoutId ??= await _interaction.CreateLogoutContextAsync();
            _identityProvider = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            await _signInManager.SignOutAsync();

            if (await ShouldTriggerExternalSignOut())
            {
                return ExternalSignOut();
            }
        }

        return await OnGetLoggedOutAsync();
    }

    public async Task<IActionResult> OnGetLoggedOutAsync()
    {
        var logout = await _interaction.GetLogoutContextAsync(_logoutId);

        var redirectUrl = logout is not null ? logout.PostLogoutRedirectUri : Urls.WebRedirect.AbsoluteUri;

        return Redirect(redirectUrl);
    }

    private async Task<bool> ShouldTriggerExternalSignOut()
    {
        if (_identityProvider is not null and not Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
        {
            return await HttpContext.SchemeSupportsSignOutAsync(_identityProvider);
        }

        return false;
    }

    private SignOutResult ExternalSignOut()
    {
        // Redirect URL to complete the single sign-out processing
        string url = Url.Page("Index", "LoggedOut", new { logoutId = _logoutId });

        return SignOut(new AuthenticationProperties { RedirectUri = url }, _identityProvider);
    }
}
