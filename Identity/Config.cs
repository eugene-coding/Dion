using Duende.IdentityServer.Models;

namespace Identity;

public static class Config
{
    static Config()
    {
        var apiScope = new ApiScope("api", "API");

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
            }
        };
    }

    public static IEnumerable<ApiScope> ApiScopes { get; private set; }
    public static IEnumerable<Client> Clients { get; private set; }
}
