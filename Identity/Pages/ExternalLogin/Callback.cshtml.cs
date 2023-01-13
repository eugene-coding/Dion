using Common;

using Duende.IdentityServer;
using Duende.IdentityServer.Events;

using Identity.Exceptions;
using Identity.Extensions;

using IdentityModel;

using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace Identity.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Callback : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<Callback> _logger;
    private readonly IEventService _events;

    public Callback(
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<Callback> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
        _logger = logger;
        _events = events;
    }

    public async Task<IActionResult> OnGet()
    {
        // read external identity from the temporary cookie
        var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (result?.Succeeded != true)
        {
            throw new ExternalAuthenticationException("External authentication error");
        }

        var externalUser = result.Principal;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var externalClaims = externalUser.Claims.Print();
            ;
            _logger.ExternalClaims(externalClaims);
        }

        // lookup our user and external provider info
        // try to determine the unique id of the external user (issued by the provider)
        // the most common claim type for that are the sub claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used
        var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new FindUserByClaimsFailedException("Unknown userid");

        var provider = result.Properties.Items["scheme"];
        var providerUserId = userIdClaim.Value;

        // find external user
        var user = await _userManager.FindByLoginAsync(provider, providerUserId);

        // this might be where you might initiate a custom workflow for user registration
        // in this sample we don't show how that would be done, as our sample implementation
        // simply auto-provisions new external user
        user ??= await AutoProvisionUserAsync(provider, providerUserId, externalUser.Claims);

        // this allows us to collect any additional claims or properties
        // for the specific protocols used and store them in the local auth cookie.
        // this is typically used to store data needed for signout from those protocols.
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

        // issue authentication cookie for user
        await _signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

        // delete temporary cookie used during external authentication
        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // retrieve return URL
        var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

        // check if external login is in the context of an OIDC request
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, user.UserName, true, context?.Client.ClientId));

        if (context != null)
        {
            if (context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(returnUrl);
            }
        }

        return Redirect(returnUrl);
    }

    private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
    {
        var sub = Guid.NewGuid().ToString();

        var user = new ApplicationUser
        {
            Id = sub,
            UserName = sub, // don't need a username, since the user will be using an external provider to login
        };

        // email
        var email = claims.GetEmail();

        if (email != null)
        {
            user.Email = email;
        }

        // create a list of claims that we want to transfer into our store
        var filtered = new List<Claim>();

        // user's display name
        var name = claims.GetName();
        if (name != null)
        {
            filtered.AddName(name);
        }
        else
        {
            var first = claims.GetFirstName();
            var last = claims.GetLastName();
            if (first != null && last != null)
            {
                filtered.AddName(first + " " + last);
            }
            else if (first != null)
            {
                filtered.AddName(first);
            }
            else if (last != null)
            {
                filtered.AddName(last);
            }
        }

        var identityResult = await _userManager.CreateAsync(user);

        if (!identityResult.Succeeded)
        {
            throw new CreateUserFailedException(identityResult.Errors.First().Description);
        }

        if (filtered.Any())
        {
            identityResult = await _userManager.AddClaimsAsync(user, filtered);

            if (!identityResult.Succeeded)
            {
                throw new AddClaimsFailedException(identityResult.Errors.First().Description);
            }
        }

        identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));

        if (!identityResult.Succeeded)
        {
            throw new AddUserLoginInfoFailedException(identityResult.Errors.First().Description);
        }

        return user;
    }

    // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
    // this will be different for WS-Fed, SAML2p or other protocols
    private static void CaptureExternalLoginContext(AuthenticateResult externalResult, IList<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        // capture the idp used to login, so the session knows where the user came from
        localClaims.AddIdentityProvider(externalResult.Properties.Items["scheme"]);

        TryAddSessionId(externalResult.Principal.Claims, localClaims);

        TryStoreIdToken(externalResult.Properties, localSignInProps);
    }

    /// <summary>
    /// If the external system sent a session id claim, copy it over
    /// so we can use it for single sign-out
    /// </summary>
    private static void TryAddSessionId(IEnumerable<Claim> externalClaims, IList<Claim> localClaims)
    {
        var sessionId = externalClaims.GetSessionId();

        if (sessionId is not null)
        {
            localClaims.AddSessionId(sessionId);
        }
    }

    /// <summary>If the external provider issued an <c>id_token</c>, we'll keep it for signout.</summary>
    private static void TryStoreIdToken(AuthenticationProperties externalProperties, AuthenticationProperties localSignInProps)
    {
        var value = externalProperties.GetTokenValue(AuthenticationTokenNames.IdToken);

        if (value is not null)
        {
            var token = new AuthenticationToken
            {
                Name = AuthenticationTokenNames.IdToken,
                Value = value
            };

            localSignInProps.StoreTokens(new[] { token });
        }
    }
}
