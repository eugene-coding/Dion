using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;

using Identity.Data;
using Identity.Exceptions;
using Identity.Models;

using IdentityModel;

using Microsoft.AspNetCore.Identity;

using Serilog;

using System.Security.Claims;

namespace Identity;

internal static class SeedData
{
    public static void InitializeIdentityServer(IServiceProvider serviceProvider)
    {
        using var context = serviceProvider.GetRequiredService<ConfigurationDbContext>();

        if (!context.Clients.Any())
        {
            foreach (var client in Config.Clients)
            {
                context.Clients.Add(client.ToEntity());
            }
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.IdentityResources)
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var resource in Config.ApiScopes)
            {
                context.ApiScopes.Add(resource.ToEntity());
            }
        }

        context.SaveChanges();
    }

    public static void InitializeAspIdentity(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var alice = userMgr.FindByNameAsync("alice").Result;

        if (alice == null)
        {
            alice = new ApplicationUser
            {
                UserName = "alice",
                Email = "AliceSmith@email.com",
                EmailConfirmed = true,
            };

            var result = userMgr.CreateAsync(alice, "Pass123$").Result;

            if (!result.Succeeded)
            {
                throw new CreateUserFailedException(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(alice, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                    }).Result;

            if (!result.Succeeded)
            {
                throw new AddClaimsFailedException(result.Errors.First().Description);
            }

            Log.Debug("alice created");
        }
        else
        {
            Log.Debug("alice already exists");
        }

        var bob = userMgr.FindByNameAsync("bob").Result;

        if (bob == null)
        {
            bob = new ApplicationUser
            {
                UserName = "bob",
                Email = "BobSmith@email.com",
                EmailConfirmed = true
            };

            var result = userMgr.CreateAsync(bob, "Pass123$").Result;

            if (!result.Succeeded)
            {
                throw new CreateUserFailedException(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(bob, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim("location", "somewhere")
                    }).Result;

            if (!result.Succeeded)
            {
                throw new AddClaimsFailedException(result.Errors.First().Description);
            }

            Log.Debug("bob created");
        }
        else
        {
            Log.Debug("bob already exists");
        }
    }
}
