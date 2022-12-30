using Duende.IdentityServer;

using Identity.Data;
using Identity.Extensions.Hosting;
using Identity.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Extensions.Hosting;

public static class IdentityExtension
{
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services, ConfigurationManager configuration)
    {
        return services.AddIdentity()
                       .AddIdentityServer(configuration)
                       .AddAuthentication(configuration);
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddIdentityServer(this IServiceCollection services, ConfigurationManager configuration)
    {        
        Action<DbContextOptionsBuilder> configureContext = options =>
        {
            var assemblyName = typeof(Program).Assembly.GetName().Name;
            var connectionString = configuration["IdentityServer:ConnectionString"];
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            options.UseMySql(connectionString, serverVersion, options =>
            {
                options.MigrationsAssembly(assemblyName);
                options.EnableRetryOnFailure();
                options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        };

        services.AddIdentityServer(options =>
        {
            options.UserInteraction.ErrorUrl = "/error";

            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            options.EmitStaticAudienceClaim = true;
        })
        .AddConfigurationStore(options =>
        {
            options.ConfigureDbContext = configureContext;
        })
        .AddOperationalStore(options =>
        {
            options.ConfigureDbContext = configureContext;
        })
        .AddAspNetIdentity<ApplicationUser>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.ClientId = configuration["Authentication:Google:ClientId"];
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            });

        return services;
    }
}
