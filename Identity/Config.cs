using Duende.IdentityServer;
using Duende.IdentityServer.Models;

using Common;

namespace Identity;

/// <summary>
/// Povides <see cref="ApiScopes">API scopes</see>, <see cref="Clients">clients</see> 
/// and <see cref="IdentityResources">identity resources</see> used in the application.
/// </summary>
public static class Config
{
    /// <summary>Gets the API scopes used in the application.</summary>
    public static IEnumerable<ApiScope> ApiScopes { get; } = new List<ApiScope>()
    {
        new (ScopeNames.Api)
    };

    /// <summary>Gets the clients used in the application.</summary>
    public static IEnumerable<Client> Clients { get; } = new List<Client>()
    {
        new Client
        {
            ClientId = Credentials.Client.Id,
            ClientSecrets = { new Secret(Credentials.Client.Secret.Sha256()) },

            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = { ScopeNames.Api }
        },

        new Client
        {
            ClientId = Credentials.Web.Id,
            ClientSecrets = { new Secret(Credentials.Web.Secret.Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,
            AllowOfflineAccess = true,

            RedirectUris = { new Uri(Urls.Web, "signin-oidc").AbsoluteUri },
            FrontChannelLogoutUri =  new Uri(Urls.Web, "signout-oidc").AbsoluteUri,
            PostLogoutRedirectUris = { new Uri(Urls.Web, "signout-callback-oidc").AbsoluteUri },

            AllowedScopes = new List<string>
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.OfflineAccess,
                ScopeNames.Api
            }
        }
    };

    /// <summary>Gets the identity resources used in the application.</summary>
    public static IEnumerable<IdentityResource> IdentityResources { get; } = new List<IdentityResource>()
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    };
}
