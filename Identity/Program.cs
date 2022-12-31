using Identity;
using Identity.Data;
using Identity.Extensions.Hosting;

using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

using Serilog;

using System.Globalization;

internal static class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().
            WriteTo.Console(formatProvider: CultureInfo.CurrentCulture)
            .CreateBootstrapLogger();

        Log.Information("Starting up");

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.ConfigureServices();

            builder.Host.ConfigureSerilog();

            var app = builder.Build();
            app.ConfigurePipeline();

            // Uncomment to seed the database
            //Log.Information("Seeding database...");
            //SeedData.Seed(app);

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
        builder.Services.AddLocalization();

        builder.Services.ConfigureCors();

        builder.Services.AddRazorPages();
        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = Session.Timeout;
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = builder.Configuration["IdentityServer:ConnectionString"];
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            options.UseMySql(connectionString, serverVersion, options =>
            {
                options.EnableRetryOnFailure();
                options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        });

        builder.Services.ConfigureIdentity(builder.Configuration);
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.ConfigureCsp();

        app.UseStaticFiles(new StaticFileOptions()
        {
            OnPrepareResponse = context =>
            {
                context.Context.Response.Headers.Add(HeaderNames.CacheControl, "public,max-age=600");
                context.Context.Response.Headers.Remove(HeaderNames.ContentSecurityPolicy);

                context.Context.Response.ContentType += ";charset=utf-8";
            }
        });

        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseSession();
        app.MapRazorPages()
           .RequireAuthorization();

        return app;
    }
}
