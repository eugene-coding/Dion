using API.Data;

using Microsoft.EntityFrameworkCore;

using Shared;

internal static class Program
{
    private const string AuthorizationPolicyName = "Api";

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
        builder.ConfigureDbContext();

        builder.Services.ConfigureAuthentication();
        builder.Services.ConfigureAuthorization();

        builder.Services.ConfigureCors();
        builder.Services.AddControllers();
    }

    private static void ConfigurePipeline(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers()
           .RequireAuthorization(AuthorizationPolicyName);
    }

    private static void ConfigureDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<Context>(options =>
        {
            var connectionString = builder.Configuration["Api:ConnectionString"];
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            options.UseMySql(connectionString, serverVersion, options =>
            {
                options.EnableRetryOnFailure()
                       .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        });
    }
    
    private static void ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(Config.BearerSchemeName)
                .AddJwtBearer(Config.BearerSchemeName, options =>
                {
                    options.Authority = Config.IdentityUrl;
                });
    }
    
    private static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicyName, policy =>
            {
                policy.RequireAuthenticatedUser()
                      .RequireClaim("scope", Config.ApiName);
            });
        });
    }
    
    private static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(Config.WebUrl)
                      .WithHeaders(Config.OidcCorsHeader);
            });
        });
    }
}
