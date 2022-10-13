using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity;

public static class Config
{
    static Config()
    {
        const string secret = "secret";
        var apiScope = new ApiScope("api", "API");

        var signInUrl = $"{Shared.Config.BffUrl}/signin-oidc";
        var signOutUrl = $"{Shared.Config.BffUrl}/signout-oidc";
        var signOutCallbackUrl = $"{Shared.Config.BffUrl}/signout-callback-oidc";

        ApiScopes = new List<ApiScope>()
        {
            apiScope
        };

        Clients = new List<Client>()
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                ClientSecrets =
                {
                    new Secret(secret.Sha256())
                },

                AllowedScopes = { apiScope.Name }
            },

            new Client
            {
                ClientId = "bff",
                
                ClientSecrets =
                {
                    new Secret(secret.Sha256())
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
                    apiScope.Name
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
