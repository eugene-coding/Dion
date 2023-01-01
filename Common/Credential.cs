namespace Common;

public sealed class Credential
{
    public Credential(string id, string secret)
    {
        Id = id;
        Secret = secret;
    }

    public string Id { get; }
    public string Secret { get; }
}
