using Duende.IdentityServer;
using Duende.IdentityServer.Models;

using Shared;

namespace Identity;

public static class Config
{
    static Config()
    {
        var signInUrl = CommonValues.WebUrl + "/signin-oidc";
        var signOutUrl = CommonValues.WebUrl + "/signout-oidc";
        var signOutCallbackUrl = CommonValues.WebUrl + "/signout-callback-oidc";

        Clients = new List<Client>()
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                AllowedScopes = { CommonValues.ApiName }
            },

            new Client
            {
                ClientId = CommonValues.WebClientId,

                ClientSecrets =
                {
                    new Secret(CommonValues.WebClientSecret.Sha256())
                },

                AllowedGrantTypes = GrantTypes.Code,
                AllowOfflineAccess = true,

                RedirectUris = { signInUrl, CommonValues.IdentityUrl + "/signin-oidc", CommonValues.IdentityUrl + "/Account/Login/Password" },
                FrontChannelLogoutUri = signOutUrl,
                PostLogoutRedirectUris = { signOutCallbackUrl },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    CommonValues.ApiName
                }
            }
        };
    }

    public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>()
        {
            new (CommonValues.ApiName)
        };
    public static IEnumerable<Client> Clients { get; private set; }
    public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>()
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };
}
