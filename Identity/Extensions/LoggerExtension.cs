namespace Identity.Extensions;

internal static class LoggerExtension
{
    private static readonly Action<ILogger, IEnumerable<string>, Exception> _externalClaims = LoggerMessage.Define<IEnumerable<string>>(
        LogLevel.Debug,
        new(1, nameof(ExternalClaims)),
        "External claims: {Claims}");

    private static readonly Action<ILogger, string, Exception> _invalidBackchannelLoginId = LoggerMessage.Define<string>(
        LogLevel.Warning,
        new(2, nameof(InvalidBackchannelLoginId)),
        "Invalid backchannel login id {Id}");

    private static readonly Action<ILogger, string, Exception> _invalidId = LoggerMessage.Define<string>(
        LogLevel.Error,
        new(3, nameof(InvalidId)),
        "Invalid id {Id}");

    private static readonly Action<ILogger, string, Exception> _noBackchannelLoginRequestMatchingId = LoggerMessage.Define<string>(
        LogLevel.Error,
        new(4, nameof(NoBackchannelLoginRequestMatchingId)),
        "No backchannel login request matching id: {Id}");

    private static readonly Action<ILogger, string, Exception> _noConsentRequestMatchingRequest = LoggerMessage.Define<string>(
        LogLevel.Error,
        new(5, nameof(NoConsentRequestMatchingRequest)),
        "No consent request matching request: {Request}");

    public static void ExternalClaims(this ILogger logger, IEnumerable<string> claims)
    {
        _externalClaims(logger, claims, null);
    }

    public static void InvalidBackchannelLoginId(this ILogger logger, string id)
    {
        _invalidBackchannelLoginId(logger, id, null);
    }

    public static void InvalidId(this ILogger logger, string id)
    {
        _invalidId(logger, id, null);
    }

    public static void NoBackchannelLoginRequestMatchingId(this ILogger logger, string id)
    {
        _noBackchannelLoginRequestMatchingId(logger, id, null);
    }

    public static void NoConsentRequestMatchingRequest(this ILogger logger, string request)
    {
        _noConsentRequestMatchingRequest(logger, request, null);
    }
}
