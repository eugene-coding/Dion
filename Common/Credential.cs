namespace Common;

/// <summary>Defines client credential.</summary>
public sealed class Credential
{
    /// <summary>Creates the <see cref="Credential"/> instance.</summary>
    /// <param name="id">Client ID.</param>
    /// <param name="secret">Client secret.</param>
    public Credential(string id, string secret)
    {
        Id = id;
        Secret = secret;
    }

    /// <summary>Gets the client ID.</summary>
    public string Id { get; }

    /// <summary>Gets the client secret.</summary>
    public string Secret { get; }
}
