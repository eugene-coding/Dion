using Duende.IdentityServer;

using Identity;
using Identity.Data;
using Identity.Models;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using Serilog;

using Common;

using HeaderNames = Microsoft.Net.Http.Headers.HeaderNames;

internal static class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console()
            .CreateBootstrapLogger();

        Log.Information("Starting up");

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.ConfigureServices();

            builder.Host.ConfigureSerilog();

            var app = builder.Build();
            app.ConfigurePipeline();

            Log.Information("Seeding database...");
            SeedData.InitializeAspIdentity(app);

            app.Run();
        }
        catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException")
        {
            Log.Fatal(ex, "Unhandled exception");
        }
        finally
        {
            Log.Information("Shut down complete");
            Log.CloseAndFlush();
        }
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var contextOptions = new DbContextOptions(builder.Configuration);

        builder.Services.AddLocalization();
        builder.Services.ConfigureCors();
        builder.Services.AddRazorPages();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.ConfigureSession();
        builder.Services.ConfigureDbContext(contextOptions);
        builder.Services.ConfigureIdentity();
        builder.Services.ConfigureIdentityServer(contextOptions);
        builder.Services.ConfigureAuthentication(builder.Configuration);
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

        app.ConfigureHeaders();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseSession();
        app.MapRazorPages()
           .RequireAuthorization();

        return app;
    }

    private static void ConfigureSession(this IServiceCollection services)
    {
        services.AddSession(options =>
        {
            options.IdleTimeout = Session.Timeout;
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    }

    private static void ConfigureDbContext(this IServiceCollection services, DbContextOptions contextOptions)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            ConfigureDbContextOptions(options, contextOptions);
        });
    }

    private static void ConfigureSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((ctx, lc) => lc
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(ctx.Configuration));
    }

    private static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(Urls.Web.AbsoluteUri)
                      .WithHeaders(HeaderNames.Authorization);
            });
        });
    }

    private static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
    }

    private static void ConfigureIdentityServer(this IServiceCollection services, DbContextOptions contextOptions)
    {
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
            options.ConfigureDbContext = options =>
            {
                ConfigureDbContextOptions(options, contextOptions);
            };
        })
        .AddOperationalStore(options =>
        {
            options.ConfigureDbContext = options =>
            {
                ConfigureDbContextOptions(options, contextOptions);
            };
        })
        .AddAspNetIdentity<ApplicationUser>();
    }

    private static void ConfigureAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.ClientId = configuration["Authentication:Google:ClientId"];
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            });
    }

    private static void ConfigureDbContextOptions(DbContextOptionsBuilder options, DbContextOptions contextOptions)
    {
        options.UseMySql(contextOptions.ConnectionString, contextOptions.ServerVersion, options =>
        {
            options.MigrationsAssembly(contextOptions.AssemblyName);
            options.EnableRetryOnFailure();
            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }

    private static void ConfigureHeaders(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            // 'unsafe-inline' only for development
            context.Response.Headers.Append("content-security-policy", "script-src 'self' 'unsafe-eval' 'unsafe-inline'");
            await next.Invoke();
        });
    }

    private struct DbContextOptions
    {
        public DbContextOptions(ConfigurationManager configuration)
        {
            ConnectionString = configuration["IdentityServer:ConnectionString"];
            ServerVersion = ServerVersion.AutoDetect(ConnectionString);

            AssemblyName = typeof(Program).Assembly.GetName().Name;
        }

        public string ConnectionString { get; set; }
        public string AssemblyName { get; set; }
        public ServerVersion ServerVersion { get; set; }
    }
}
