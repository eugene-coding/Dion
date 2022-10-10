using Shared;

using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllersWithViews();
services.AddRazorPages();

const string defaultSchemeName = "Cookies";
const string defaultChallengeSchemeName = "oidc";

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

services.AddAuthentication(options =>
{
    options.DefaultScheme = defaultSchemeName;
    options.DefaultChallengeScheme = defaultChallengeSchemeName;
})
    .AddCookie(defaultSchemeName)
    .AddOpenIdConnect(defaultChallengeSchemeName, options =>
    {
        options.Authority = Config.IdentityUrl;

        options.ClientId = "user";
        options.ClientSecret = "secret";
        options.ResponseType = "code";

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");

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
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
