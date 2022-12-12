namespace Common;

/// <summary>Provides the URL of the projects in the solution.</summary>
public static class Urls
{
    private readonly static Uri _base = new("https://localhost");

    /// <summary>URL of the Identity project.</summary>
    public static Uri Identity => new UriBuilder(_base) { Port = 5001 }.Uri;

    /// <summary>URL of the Web project.</summary>
    public static Uri Web => new UriBuilder(_base) { Port = 5002 }.Uri;
}
