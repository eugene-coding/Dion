using IdentityModel.Client;

using Microsoft.AspNetCore.Components;

using System.Text.Json;

namespace UI.Shared;

public partial class TestIdentityServer
{
    private string? _disco;
    private string? _tokenValue;
    private TokenResponse? _token;
    private string? _apiResult;

    [Inject]
    public HttpClient Client { get; init; } = default!;

    public async Task GetAsync()
    {
        var disco = await Client.GetDiscoveryDocumentAsync("https://localhost:5001");

        if (disco.IsError)
        {
            _disco = disco.Error;
        }

        _token = await Client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,

            ClientId = "client",
            ClientSecret = "secret",
            Scope = "api"
        });

        _tokenValue = _token.IsError ? _token.Error : _token.AccessToken;

        await GetApiAsync();
    }

    public async Task GetApiAsync()
    {
        Client.SetBearerToken(_token?.AccessToken);

        var response = await Client.GetAsync("https://localhost:6001/user");
        
        if (!response.IsSuccessStatusCode)
        {
            _apiResult = response.StatusCode.ToString();
        }
        else
        {
            var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
            _apiResult = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
