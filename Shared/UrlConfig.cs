namespace Shared;

public static class UrlConfig
{
    private readonly static Uri _base = new("https://localhost");

    public static string IdentityUrl => new UriBuilder(_base) { Port = 5001 }.ToString();
    public static string WebUrl => new UriBuilder(_base) { Port = 5002 }.ToString();
}
