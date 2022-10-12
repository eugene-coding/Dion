using API.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Shared;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<Context>(options =>
{
    var connectionString = builder.Configuration["Api:ConnectionString"];
    var serverVersion = ServerVersion.AutoDetect(connectionString);

    options.UseMySql(connectionString, serverVersion, options =>
    {
        options.EnableRetryOnFailure();
    });
});

services.AddAuthentication(Config.BearerSchemeName)
    .AddJwtBearer(Config.BearerSchemeName, options =>
    {
        options.Authority = Config.IdentityUrl;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

services.AddAuthorization(options =>
{
    options.AddPolicy("Api", policy =>
    {
        policy
            .RequireAuthenticatedUser()
            .RequireClaim("scope", "api");
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(Config.WebUrl)
            .WithHeaders(Config.OidcCorsHeader);
    });
});

services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    SeedData.Initialize(serviceProvider);
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization("Api");

app.Run();
