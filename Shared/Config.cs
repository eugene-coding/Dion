namespace Shared;

public static class Config
{
    private readonly static string _baseUrl = "https://localhost";

    public static string ApiUrl => $"{_baseUrl}:6001";
    public static string IdentityUrl => $"{_baseUrl}:5001";
    public static string WebUrl => $"{_baseUrl}:5002";
}
