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
    public static TimeSpan SessionTimeout => TimeSpan.FromMinutes(100);
}
