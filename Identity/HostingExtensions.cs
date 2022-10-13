using Duende.IdentityServer;

using Identity.Data;
using Identity.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Serilog;

namespace Identity;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddRazorPages();

        var optionsSettings = new OptionsSettings(builder);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            ConfigureDbContextOptions(options, optionsSettings);
        });

        services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            options.EmitStaticAudienceClaim = true;
        })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = options =>
                {
                    ConfigureDbContextOptions(options, optionsSettings);
                };
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = options =>
                {
                    ConfigureDbContextOptions(options, optionsSettings);
                };
            })
            .AddAspNetIdentity<ApplicationUser>();

        services.AddAuthentication()
            .AddGoogle(options =>
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
            SeedData.InitializeIdentityServer(serviceProvider);
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
           .RequireAuthorization();

        return app;
    }

    private static void ConfigureDbContextOptions(DbContextOptionsBuilder options, OptionsSettings settings)
    {
        options.UseMySql(settings.ConnectionString, settings.ServerVersion, options =>
        {
            options.MigrationsAssembly(settings.AssemblyName);
            options.EnableRetryOnFailure();
            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }

    private struct OptionsSettings
    {
        public OptionsSettings(WebApplicationBuilder builder)
        {
            ConnectionString = builder.Configuration["IdentityServer:ConnectionString"];
            ServerVersion = ServerVersion.AutoDetect(ConnectionString);

            AssemblyName = typeof(Program).Assembly.GetName().Name;
        }

        public string ConnectionString { get; set; }
        public ServerVersion ServerVersion { get; set; }
        public string AssemblyName { get; set; }
    }
}
