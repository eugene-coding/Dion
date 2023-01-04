namespace Common;

/// <summary>Provides the URL of the projects in the solution.</summary>
public static class Urls
{
    private readonly static Uri _base = new("https://localhost");

    /// <summary>URL of the Identity project.</summary>
    public static Uri Identity { get; } = new UriBuilder(_base) { Port = 5001 }.Uri;

    /// <summary>URL of the Web project.</summary>
    public static Uri Web { get; } = new UriBuilder(_base) { Port = 5002 }.Uri;

    /// <summary>URL of the Web project`s Redirect page.</summary>
    public static Uri WebRedirect { get; } = new UriBuilder(Web) { Path = "Redirect" }.Uri;
}
