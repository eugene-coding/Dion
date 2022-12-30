using Common;

using HeaderNames = Microsoft.Net.Http.Headers.HeaderNames;

namespace Identity.Extensions.Hosting;

public static class HeaderExtension
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(Urls.Web.AbsoluteUri)
                      .WithHeaders(HeaderNames.Authorization);
            });
        });

        return services;
    }

    public static WebApplication ConfigureCsp(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var value = "script-src 'self' 'unsafe-eval'";
            
            if (app.Environment.IsDevelopment())
            {
                value += " 'unsafe-inline'";
            }

            context.Response.Headers.Append(HeaderNames.ContentSecurityPolicy, value);

            await next.Invoke();
        });

        return app;
    }
}
