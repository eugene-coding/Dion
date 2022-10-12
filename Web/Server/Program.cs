using Duende.IdentityServer;

using Shared;

using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllersWithViews();
services.AddRazorPages();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddBff();

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

        options.ClientId = "user";
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

var app = builder.Build();

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
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
