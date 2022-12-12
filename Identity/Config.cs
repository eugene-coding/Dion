using Duende.IdentityServer;
using Duende.IdentityServer.Models;

using Common;

namespace Identity;

public static class Config
{
    static Config()
    {
        var signIn = new Uri(Urls.Web, "signin-oidc").ToString();
        var signOut = new Uri(Urls.Web, "signout-oidc").ToString();
        var signOutCallback = new Uri(Urls.Web, "signout-callback-oidc").ToString();

        Clients = new List<Client>()
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

                RedirectUris = { signIn },
                FrontChannelLogoutUri = signOut,
                PostLogoutRedirectUris = { signOutCallback },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    ScopeNames.Api
                }
            }
        };
    }

    public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>()
    {
        new (ScopeNames.Api)
    };
    public static IEnumerable<Client> Clients { get; private set; }
    public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>()
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    };
}
