namespace Common;

/// <summary>Specifies the client credentials used by the application.</summary>
public static class Credentials
{
    /// <summary>Credentials of the Client client.</summary>
    public static Credential Client => new("client", "secret");

    public static Credential Web => new("bff", "secret");
    /// <summary>Credentials of the Web client.</summary>
}
