using Common;

using System.Text.RegularExpressions;

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

    public static void SetUtf8Charset(this HttpResponse response)
    {
        const string pattern = "(application\\/(javascript|json|rss\\+xml|manifest\\+json)" +
                               "|image\\/svg\\+xml" +
                               "|text\\/(xml|javascript|cache-manifest|css|html|plain|vtt))";

        if (Regex.IsMatch(response.ContentType, pattern))
        {
            response.ContentType += "; charset=utf-8";
        }
    }
}
