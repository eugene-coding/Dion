namespace Web.Client.Extensions;

internal static class LoggerExtension
{
    private static readonly Action<ILogger, Exception?> _takingUserFromCache = LoggerMessage.Define(
        LogLevel.Debug,
        new(1, nameof(TakingUserFromCache)),
        "Taking user from cache");

    private static readonly Action<ILogger, Exception?> _fetchingUser = LoggerMessage.Define(
        LogLevel.Debug,
        new(2, nameof(FetchingUser)),
        "Fetching user");

    private static readonly Action<ILogger, Exception?> _fetchingUserInformation = LoggerMessage.Define(
        LogLevel.Information,
        new(3, nameof(FetchingUserInformation)),
        "Fetching user information");

    private static readonly Action<ILogger, Exception?> _fetchingUserFailed = LoggerMessage.Define(
        LogLevel.Warning,
        new(4, nameof(FetchingUserFailed)),
        "Fetching user failed");

    public static void TakingUserFromCache(this ILogger logger)
    {
        _takingUserFromCache(logger, null);
    }

    public static void FetchingUser(this ILogger logger)
    {
        _fetchingUser(logger, null);
    }

    public static void FetchingUserInformation(this ILogger logger)
    {
        _fetchingUserInformation(logger, null);
    }

    public static void FetchingUserFailed(this ILogger logger, Exception exception)
    {
        _fetchingUserFailed(logger, exception);
    }
}
