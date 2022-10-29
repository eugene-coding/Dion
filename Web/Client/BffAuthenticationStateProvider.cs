using Microsoft.AspNetCore.Components.Authorization;

using System.Net.Http.Json;
using System.Security.Claims;

namespace Web.Client;

public class BffAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60);

    private readonly HttpClient _client;
    private readonly ILogger<BffAuthenticationStateProvider> _logger;

    private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0);
    private ClaimsPrincipal _cachedUser = new(new ClaimsIdentity());

    public BffAuthenticationStateProvider(HttpClient client, ILogger<BffAuthenticationStateProvider> logger)
    {
        _client = client;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUser();

        return new(user);
    }

    private async ValueTask<ClaimsPrincipal> GetUser(bool useCache = true)
    {
        var now = DateTimeOffset.Now;

        if (useCache && now < _userLastCheck + _userCacheRefreshInterval)
        {
            _logger.LogDebug("Taking user from cache");

            return _cachedUser;
        }

        _logger.LogDebug("Fetching user");
        _cachedUser = await FetchUser();
        _userLastCheck = now;

        return _cachedUser;
    }

    private async Task<ClaimsPrincipal> FetchUser()
    {
        try
        {
            _logger.LogInformation("Fetching user information");

            var claimRecords = await _client.GetFromJsonAsync<List<ClaimRecord>>("bff/user?slide=false");

            if (claimRecords is not null)
            {
                var identity = new ClaimsIdentity(nameof(BffAuthenticationStateProvider), "name", "role");

                foreach (var claimRecord in claimRecords)
                {
                    var claim = new Claim(claimRecord.Type, claimRecord.Value.ToString()!);

                    identity.AddClaim(claim);
                }

                return new(identity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Fetching user failed");
        }

        return new(new ClaimsIdentity());
    }

    record ClaimRecord(string Type, object Value);
}
