using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;

using Identity.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Identity.Pages.Login;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityProviderStore _identityProviderStore;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly UserManager<ApplicationUser> _userManager;

    public Index(
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IIdentityServerInteractionService interaction,
        IStringLocalizer<Index> text,
        UserManager<ApplicationUser> userManager)
    {
        _schemeProvider = schemeProvider;
        _identityProviderStore = identityProviderStore;
        _interaction = interaction;
        _userManager = userManager;

        Text = text;
    }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; init; }

    [BindProperty]
    public InputModel Input { get; init; } = new();

    public string SubmitButtonId => "submit";
    public IStringLocalizer<Index> Text { get; private init; }
    public ViewModel View { get; private init; } = new();

    public async Task<IActionResult> OnGet()
    {
        var context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);

        if (context?.IdP is not null && await _schemeProvider.GetSchemeAsync(context.IdP) is not null)
        {
            return BuildViewModelFromIdP(context);
        }

        return await BuildViewModel(context);
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
        return LocalRedirect("/Account/Login/Password" + query);
    }

    private IActionResult BuildViewModelFromIdP(AuthorizationRequest context)
    {
        var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

        Input.Username = context?.LoginHint;

        if (!local)
        {
            View.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
        }

        return Page();
    }

    private async Task<IActionResult> BuildViewModel(AuthorizationRequest context)
    {
        var providers = await GetExternalProvidersAsync();

        var client = context?.Client;

        if (client is not null)
        {
            if (client.IdentityProviderRestrictions is not null && client.IdentityProviderRestrictions.Any())
            {
                providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
            }
        }

        View.ExternalProviders = providers.ToArray();

        return Page();
    }

    private async Task<IEnumerable<ExternalProvider>> GetExternalProvidersAsync()
    {
        var schemes = await _schemeProvider.GetAllSchemesAsync();
        var identityProviderNames = await _identityProviderStore.GetAllSchemeNamesAsync();

        var providers = ExternalProvider.GetExternalProviders(schemes, identityProviderNames);

        return providers;
    }
}
