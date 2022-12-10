namespace Identity;

/// <summary>Specifies session settings.</summary>
public static class Session
{
    /// <summary>
    /// Gets the amount of time allowed between requests 
    /// before the session-state provider terminates the session.
    /// </summary>
    public static TimeSpan Timeout => TimeSpan.FromMinutes(20);    
}
