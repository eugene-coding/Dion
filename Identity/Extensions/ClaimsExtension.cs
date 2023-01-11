using IdentityModel;

using System.Collections.Generic;
using System.Security.Claims;

namespace Identity.Extensions;

public static class ClaimsExtension
{
    public static IEnumerable<string> Print(this IEnumerable<Claim> claims)
    {
        return claims.Select(c => $"{c.Type}: {c.Value}");
    }

    public static void AddName(this ICollection<Claim> claims, string name)
    {
        claims.Add(new Claim(JwtClaimTypes.Name, name));
    }

    public static void AddIdentityProvider(this ICollection<Claim> claims, string identityProvider)
    {
        claims.Add(new Claim(JwtClaimTypes.IdentityProvider, identityProvider));
    }

    public static void AddSessionId(this ICollection<Claim> claims, string sessionId)
    {
        claims.Add(new Claim(JwtClaimTypes.SessionId, sessionId));
    }

    public static string GetEmail(this IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
    }

    public static string GetName(this IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
    }

    public static string GetFirstName(this IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
    }

    public static string GetLastName(this IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
    }

    public static string GetSessionId(this IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId)?.Value;
    }
}
