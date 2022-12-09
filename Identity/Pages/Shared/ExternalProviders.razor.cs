using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;

namespace Identity.Pages.Shared;

public sealed partial class ExternalProviders
{
    private AuthorizationRequest _context;
    private IEnumerable<Provider> _providers = Enumerable.Empty<Provider>();

    [Inject] private LinkGenerator LinkGenerator { get; init; }
    [Inject] private IAuthenticationSchemeProvider SchemeProvider { get; init; }
    [Inject] private IIdentityProviderStore IdentityProviderStore { get; init; }
    [Inject] private IIdentityServerInteractionService Interaction { get; init; }

    [Parameter]
    public string ReturnUrl { get; init; }

    protected override async Task OnInitializedAsync()
    {
        _context = await Interaction.GetAuthorizationContextAsync(ReturnUrl);

        if (_context?.IdP is not null && await SchemeProvider.GetSchemeAsync(_context.IdP) is not null)
        {
            if (_context.IdP != IdentityServerConstants.LocalIdentityProvider)
            {
                _providers = GetRequestedProvider();

                return;
            }
        }

        _providers = await GetVisibleProviders();
    }

    private List<Provider> GetRequestedProvider()
    {
        return new List<Provider>
        {
            new Provider
            {
                AuthenticationScheme = _context.IdP
            }
        };
    }

    private async Task<List<Provider>> GetVisibleProviders()
    {
        var providers = await GetProviders();

        var client = _context?.Client;

        if (client?.IdentityProviderRestrictions is not null && client.IdentityProviderRestrictions.Any())
        {
            providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme));
        }

        providers = providers.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        return providers.ToList();
    }

    private async Task<IEnumerable<Provider>> GetProviders()
    {
        var schemes = await SchemeProvider.GetAllSchemesAsync();
        var identityProviderNames = await IdentityProviderStore.GetAllSchemeNamesAsync();

        var providers = GetProviders(schemes);
        var dynamicSchemes = GetProviders(identityProviderNames);

        providers = providers.Concat(dynamicSchemes);

        return providers;
    }

    private Uri GetProviderLink(string authenticationScheme)
    {
        var uriString = LinkGenerator.GetPathByPage("/ExternalLogin/Challenge", values: new { scheme = authenticationScheme, returnUrl = ReturnUrl });
        var providerLink = new Uri(uriString, UriKind.Relative);

        return providerLink;
    }

    private static IEnumerable<Provider> GetProviders(IEnumerable<AuthenticationScheme> schemes)
    {
        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new Provider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            });

        return providers;
    }

    private static IEnumerable<Provider> GetProviders(IEnumerable<IdentityProviderName> identityProviderNames)
    {
        var providers = identityProviderNames
            .Where(x => x.Enabled)
            .Select(x => new Provider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName
            });

        return providers;
    }

    private sealed class Provider
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }
    }
}
