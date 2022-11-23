namespace Shared;

public static class Config
{
    public const string ApiName = "api";
    public const string BearerSchemeName = "bearer";
    public const string CookieSchemeName = "cookies";
    public const string OidcSchemeName = "oidc";
    public const string OidcCorsHeader = "authorization";
    public const string WebClientId = "bff";
    public const string WebClientSecret = "secret";
    private const string BaseUrl = "https://localhost";

    public static string ApiUrl => $"{BaseUrl}:6001";
    public static string IdentityUrl => $"{BaseUrl}:5001";
    public static string WebUrl => $"{BaseUrl}:5002";
}
