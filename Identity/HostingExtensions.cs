using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;

using Identity.Data;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace Identity;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddRazorPages();

        var connectionString = builder.Configuration["IdentityServer:ConnectionString"];

        services.AddIdentityServer()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => ConfigureDbContext(builder, connectionString);
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => ConfigureDbContext(builder, connectionString);
            })
            .AddTestUsers(TestUsers.Users);

        services.AddAuthentication()
            .AddGoogle("Google", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;

            SeedData.Initialize(serviceProvider);
        }

        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer();

        app.UseAuthorization();

        app
            .MapRazorPages()
            .RequireAuthorization();

        return app;
    }

    private static void ConfigureDbContext(DbContextOptionsBuilder builder, string connectionString)
    {
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        var migrationAssembly = typeof(Program).Assembly.GetName().Name;

        builder.UseMySql(connectionString, serverVersion, options =>
        {
            options.MigrationsAssembly(migrationAssembly);
        });
    }
}
