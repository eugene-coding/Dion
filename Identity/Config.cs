using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity;

public static class Config
{
    static Config()
    {
        var apiScope = new ApiScope("api", "API");
        
        var clientUrl = new Uri("https://localhost:5002");
        var signInUrl = new Uri($"{clientUrl}/signin-oidc");
        var signOutUrl = new Uri($"{clientUrl}/signout-callback-oidc");

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
                    new Secret("secret".Sha256())
                },

                AllowedScopes = { apiScope.Name }
            },

            new Client
            {
                ClientId = "user",

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { signInUrl.ToString() },
                PostLogoutRedirectUris = { signOutUrl.ToString() },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
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
