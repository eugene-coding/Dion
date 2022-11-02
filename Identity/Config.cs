using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity;

public static class Config
{
    static Config()
    {
        var signInUrl = $"{Shared.Config.WebUrl}/signin-oidc";
        var signOutUrl = $"{Shared.Config.WebUrl}/signout-oidc";
        var signOutCallbackUrl = $"{Shared.Config.WebUrl}/signout-callback-oidc";

        ApiScopes = new List<ApiScope>()
        {
            new (Shared.Config.ApiName, "API")
        };

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

                AllowedScopes = { Shared.Config.ApiName }
            },

            new Client
            {
                ClientId = Shared.Config.WebClientId,

                ClientSecrets =
                {
                    new Secret(Shared.Config.WebClientSecret.Sha256())
                },

                AllowedGrantTypes = GrantTypes.Code,
                AllowOfflineAccess = true,

                RedirectUris = { signInUrl },
                FrontChannelLogoutUri = signOutUrl,
                PostLogoutRedirectUris = { signOutCallbackUrl },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    Shared.Config.ApiName
                }
            }
        };

        IdentityResources = new List<IdentityResource>()
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };
    }

    public static IEnumerable<ApiScope> ApiScopes { get; private set; }
    public static IEnumerable<Client> Clients { get; private set; }
    public static IEnumerable<IdentityResource> IdentityResources { get; private set; }
}
