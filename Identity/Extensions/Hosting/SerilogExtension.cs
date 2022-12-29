using Serilog;

using System.Globalization;

namespace Identity.Extensions.Hosting;

public static class SerilogExtension
{
    public static IHostBuilder ConfigureSerilog(this ConfigureHostBuilder host)
    {
        const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

        return host.UseSerilog((ctx, lc) => lc
                   .WriteTo.Console(
                        outputTemplate: outputTemplate,
                        formatProvider: CultureInfo.CurrentCulture)
                   .Enrich.FromLogContext()
                   .ReadFrom.Configuration(ctx.Configuration));
    }
}
