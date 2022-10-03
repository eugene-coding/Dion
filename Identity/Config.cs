using Duende.IdentityServer.Models;

namespace Identity;

public static class Config
{
    private const string _apiScopeName = "api";

    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
        new IdentityResources.OpenId()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new(_apiScopeName, "API")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                AllowedScopes = {_apiScopeName}
            }
        };
}
