using Duende.IdentityServer.Models;

using Microsoft.AspNetCore.Authentication;

namespace Identity.Pages.Login;

public sealed class ExternalProvider
{
    public string DisplayName { get; set; }
    public string AuthenticationScheme { get; set; }

    public static IEnumerable<ExternalProvider> GetExternalProviders(IEnumerable<AuthenticationScheme> schemes, IEnumerable<IdentityProviderName> identityProviderNames)
    {
        var schemesProviders = GetExternalProviders(schemes);
        var identityProviders = GetExternalProviders(identityProviderNames);

        var providers = schemesProviders.Concat(identityProviders);

        return providers;
    }

    private static IEnumerable<ExternalProvider> GetExternalProviders(IEnumerable<AuthenticationScheme> schemes)
    {
        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            });

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
}
