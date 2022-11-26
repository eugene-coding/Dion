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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityProviderStore _identityProviderStore;

    public string SubmitButtonId => "submit";
    public ViewModel View { get; set; } = new();
    public IStringLocalizer<Index> Text { get; private init; }

    [BindProperty]
    public InputModel Input { get; set; }

    public Index(
        IIdentityServerInteractionService interaction,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IStringLocalizer<Index> text,
        UserManager<ApplicationUser> userManager)

    {
        _userManager = userManager;
        _interaction = interaction;
        _schemeProvider = schemeProvider;
        _identityProviderStore = identityProviderStore;

        Text = text;
    }

    public async Task<IActionResult> OnGet(string returnUrl)
    {
        Input = new()
        {
            ReturnUrl = returnUrl
        };

        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

        if (context?.IdP is not null && await _schemeProvider.GetSchemeAsync(context.IdP) is not null)
        {
            return BuildViewModelFromIdP(context, returnUrl);
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
        return Redirect("/Account/Login/Password" + query);
    }

    private IActionResult BuildViewModelFromIdP(AuthorizationRequest context, string returnUrl)
    {
        var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

        View = new();

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

        View = new()
        {
            ExternalProviders = providers.ToArray()
        };

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
