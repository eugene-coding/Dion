namespace Shared;

public static class Config
{
    public static string ApiName => "api";
    public static string BearerSchemeName => "bearer";
    public static string CookieSchemeName => "cookies";
    public static string OidcSchemeName => "oidc";
    public static string OidcCorsHeader => "authorization";
    public static string WebClientId => "user";
    public static string WebClientSecret => "secret";
    private static string BaseUrl => "https://localhost";


    public static string ApiUrl => $"{BaseUrl}:6001";
    public static string IdentityUrl => $"{BaseUrl}:5001";
    public static string WebUrl => $"{BaseUrl}:5002";
    public static TimeSpan SessionTimeout => TimeSpan.FromSeconds(100);
}
