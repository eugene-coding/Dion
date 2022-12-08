namespace Shared;

public static class Urls
{
    private readonly static Uri _base = new("https://localhost");

    public static Uri Identity => new UriBuilder(_base) { Port = 5001 }.Uri;
    public static Uri Web => new UriBuilder(_base) { Port = 5002 }.Uri;
}
