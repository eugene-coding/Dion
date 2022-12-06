namespace Shared;

public static class CommonValues
{
    public static string ApiName => "api";
    public static string BearerSchemeName => "bearer";
    public static string CookieSchemeName => "cookies";
    public static string OidcSchemeName => "oidc";
    public static string OidcCorsHeader => "authorization";
    public static string WebClientId => "bff";
    public static string WebClientSecret => "secret";
    private static string BaseUrl => "https://localhost";

    public static string ApiUrl => $"{BaseUrl}:6001";
    public static string IdentityUrl => $"{BaseUrl}:5001";
    public static string WebUrl => $"{BaseUrl}:5002";
    public static string AuthenticationRedirectUrl => $"{WebUrl}/AuthorizeRedirect";
    public static TimeSpan SessionTimeout => TimeSpan.FromMinutes(100);
}
