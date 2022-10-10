namespace Shared;

public static class Config
{
    public const string CookieSchemeName = "cookies";
    public const string OidcSchemeName = "oidc";
    public const string OidcCorsHeader = "authorization";

    private readonly static string _baseUrl = "https://localhost";

    public static string ApiUrl => $"{_baseUrl}:6001";
    public static string IdentityUrl => $"{_baseUrl}:5001";
    public static string WebUrl => $"{_baseUrl}:5002";
}
