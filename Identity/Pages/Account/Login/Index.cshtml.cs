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

using Shared;

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Identity.Pages.Login;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityProviderStore _identityProviderStore;
    private readonly IIdentityServerInteractionService _interaction;

    public Index(
        UserManager<ApplicationUser> userManager,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IIdentityServerInteractionService interaction,
        IStringLocalizer<Index> text)
    {
        _schemeProvider = schemeProvider;
        _identityProviderStore = identityProviderStore;
        _interaction = interaction;
        _userManager = userManager;

        Text = text;
    }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; init; }

    [Required]
    [BindProperty]
    [Display(Name = nameof(Username))]
    [PageRemote(
        AdditionalFields = "__RequestVerificationToken",
        ErrorMessage = "Username doesn`t exist", 
        HttpMethod = WebRequestMethods.Http.Post, 
        PageHandler = "ValidateUsername")]
    public string Username { get; set; }

    public string SubmitButtonId => "submit";
    public IStringLocalizer<Index> Text { get; private init; }
    public ViewModel View { get; private init; } = new();

    private ISession Session => HttpContext.Session;

    public async Task<IActionResult> OnGet()
    {
        if (!string.IsNullOrEmpty(Session.GetString(SessionKeys.Username)))
        {
            Username = Session.GetString(SessionKeys.Username);
        }

        var context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);

        if (context?.IdP is not null && await _schemeProvider.GetSchemeAsync(context.IdP) is not null)
        {
            return BuildViewModelFromIdP(context);
        }

        return await BuildViewModel(context);
    }

    public async Task<JsonResult> OnPostValidateUsernameAsync()
    {
        var trimmedUsername = Username.Trim();

        var user = await _userManager.FindByNameAsync(trimmedUsername);
        var valid = user is not null;

        if (valid)
        {
            Session.SetString(SessionKeys.Username, trimmedUsername);
        }

        return new JsonResult(valid);
    }

    public IActionResult OnGetSuccess(string query)
    {
        return LocalRedirect("~/Account/Login/Password" + query);
    }

    private IActionResult BuildViewModelFromIdP(AuthorizationRequest context)
    {
        var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

        Username = context?.LoginHint;

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
