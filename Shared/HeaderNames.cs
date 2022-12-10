namespace Shared;

/// <summary>
/// Defines constants for well-known HTTP headers
/// that are not present in the <see cref="Microsoft.Net.Http.Headers.HeaderNames"/>.
/// </summary>
public static class HeaderNames
{
    /// <summary>Gets the <c>Refresh</c> HTTP header name.</summary>
    public const string Refresh = "Refresh";

    /// <summary>Gets the <c>X-CSRF</c> HTTP header name.</summary>
    public const string XCSRF = "X-CSRF";
}
