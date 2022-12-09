namespace Shared;

public static class Credentials
{
    public static Credential Client => new ("client", "secret");
    public static Credential Web => new("user", "secret");
}
