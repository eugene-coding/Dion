using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;

namespace Identity.Pages.Shared;

public sealed partial class ExternalProviders
{
    private IEnumerable<ExternalProvider> _externalProviders = Enumerable.Empty<ExternalProvider>();

    [Inject] private IAuthenticationSchemeProvider SchemeProvider { get; init; }
    [Inject] private IIdentityProviderStore IdentityProviderStore { get; init; }
    [Inject] private IIdentityServerInteractionService Interaction { get; init; }

    [Parameter] public string ReturnUrl { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        var context = await Interaction.GetAuthorizationContextAsync(ReturnUrl);

        if (context?.IdP is not null && await SchemeProvider.GetSchemeAsync(context.IdP) is not null)
        {
            var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

            if (!local)
            {
                _externalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
            }

            return;
        }

        _externalProviders = await GetExternalProviders();

        var client = context?.Client;

        if (client?.IdentityProviderRestrictions is not null && client.IdentityProviderRestrictions.Any())
        {
            _externalProviders = _externalProviders.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
        }

        _externalProviders = _externalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName)).ToList();
    }

    private async Task<List<ExternalProvider>> GetExternalProviders()
    {
        var schemes = await SchemeProvider.GetAllSchemesAsync();
        var identityProviderNames = await IdentityProviderStore.GetAllSchemeNamesAsync();

        var providers = GetExternalProviders(schemes);
        var dynamicSchemes = GetExternalProviders(identityProviderNames);

        providers.AddRange(dynamicSchemes);

        return providers;
    }

    private static List<ExternalProvider> GetExternalProviders(IEnumerable<AuthenticationScheme> schemes)
    {
        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        return providers;
    }

    private static IEnumerable<ExternalProvider> GetExternalProviders(IEnumerable<IdentityProviderName> identityProviderNames)
    {
        var providers = identityProviderNames
            .Where(x => x.Enabled)
            .Select(x => new ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName
            });

        return providers;
    }

    private sealed class ExternalProvider
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }
    }
}
