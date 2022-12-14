namespace Common;

/// <summary>Specifies the client credentials used by the application.</summary>
public static class Credentials
{
    /// <summary>Credentials of the Client client.</summary>
    public static Credential Client { get; } = new("client", "secret");

    /// <summary>Credentials of the Web client.</summary>
    public static Credential Web { get; } = new("bff", "secret");
}
