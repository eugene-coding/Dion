using Duende.IdentityServer;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Shared;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        app.ConfigurePipeline();

        app.Run();
    }

    private static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        builder.Services.AddBff();
        builder.Services.ConfigureAuthentication();
    }

    private static void ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseBff();
        app.UseAuthorization();

        app.MapBffManagementEndpoints();
        app.MapRazorPages().RequireAuthorization();

        app.MapControllers()
           .RequireAuthorization()
           .AsBffApiEndpoint();

        app.MapFallbackToFile("index.html");
    }

    private static void ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = Config.CookieSchemeName;
            options.DefaultChallengeScheme = Config.OidcSchemeName;
            options.DefaultSignOutScheme = Config.OidcSchemeName;
        })
            .AddCookie(Config.CookieSchemeName, options =>
            {
                options.Cookie.Name = "__Host-blazor";
                options.Cookie.SameSite = SameSiteMode.Strict;
            })
            .AddOpenIdConnect(Config.OidcSchemeName, options =>
            {
                options.Authority = Config.IdentityUrl;
                options.ClientId = "bff";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.ResponseMode = "query";
                options.Scope.Clear();
                options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                options.Scope.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
                options.Scope.Add("api");
                options.MapInboundClaims = false;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;
            });
    }
}
