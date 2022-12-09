namespace Shared;

public class Credential
{
    public Credential(string id, string secret)
    {
        Id = id;
        Secret = secret;
    }

    public string Id { get; private init; }
    public string Secret { get; private init; }
}
