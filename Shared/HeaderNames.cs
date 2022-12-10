namespace Shared;

/// <summary>
/// Defines constants for well-known HTTP headers
/// that are not present in the <see cref="Microsoft.Net.Http.Headers.HeaderNames"/>.
/// </summary>
public static class HeaderNames
{
    /// <summary>Gets the <c>Content-Security-Policy</c> HTTP header name.</summary>
    public const string ContentSecurityPolicy = "Content-Security-Policy";

    /// <summary>Gets the <c>Referrer-Policy</c> HTTP header name.</summary>
    public const string ReferrerPolicy = "Referrer-Policy";

    /// <summary>Gets the <c>Refresh</c> HTTP header name.</summary>
    public const string Refresh = "Refresh";

    /// <summary>Gets the <c>X-Content-Security-Policy</c> HTTP header name.</summary>
    public const string XContentSecurityPolicy = "X-Content-Security-Policy";

    /// <summary>Gets the <c>X-Content-Type-Options</c> HTTP header name.</summary>
    public const string XContentTypeOptions = "X-Content-Type-Options";

    /// <summary>Gets the <c>X-Frame-Options</c> HTTP header name.</summary>
    public const string XFrameOptions = "X-Frame-Options";

    /// <summary>Gets the <c>X-CSRF</c> HTTP header name.</summary>
    public const string XCSRF = "X-CSRF";
}
