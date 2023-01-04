using Common;

using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;

using Identity.Extensions;
using Identity.Models;

using IdentityModel;

using Microsoft.AspNetCore.Identity;

namespace Identity.Pages;

[SecurityHeaders]
[AllowAnonymous]
public class LogoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;

    [BindProperty(SupportsGet = true)]
    public string LogoutId { get; set; }

    public LogoutModel(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction, IEventService events)
    {
        _signInManager = signInManager;
        _interaction = interaction;
        _events = events;
    }

    public async Task<IActionResult> OnGet()
    {
        if (User?.Identity.IsAuthenticated == true)
        {
            // if there's no current logout context, we need to create one
            // this captures necessary info from the current logged in user
            // this can still return null if there is no context needed
            LogoutId ??= await _interaction.CreateLogoutContextAsync();

            // delete local authentication cookie
            await _signInManager.SignOutAsync();

            // raise the logout event
            await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

            // see if we need to trigger federated logout
            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            // if it's a local login we can ignore this workflow
            if (idp is not null and not Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
            {
                // we need to see if the provider supports external logout
                if (await HttpContext.GetSchemeSupportingSignOutAsync(idp))
                {
                    // build a return URL so the upstream provider will redirect back
                    // to us after the user has logged out. this allows us to then
                    // complete our single sign-out processing.
                    string url = Url.Page("Index", "Logout", new { logoutId = LogoutId });

                    // this triggers a redirect to the external provider for sign-out
                    return SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
                }
            }
        }

        return await OnGetLogoutAsync();
    }

    public async Task<IActionResult> OnGetLogoutAsync()
    {
        var logout = await _interaction.GetLogoutContextAsync(LogoutId);

        var redirectUrl = logout is not null ? logout.PostLogoutRedirectUri : Urls.WebRedirect.AbsoluteUri;

        return Redirect(redirectUrl);
    }
}
