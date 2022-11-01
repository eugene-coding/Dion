using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using UI;

using Web.Client;

internal static class Program
{
    private const string ClientName = "backend";

    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.ConfigureRootComponents();
        builder.ConfigureServices();

        await builder.Build().RunAsync();
    }

    private static void ConfigureRootComponents(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
    }

    private static void ConfigureServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();

        builder.Services.ConfigureHttpClient(builder.HostEnvironment.BaseAddress);
    }

    private static void ConfigureHttpClient(this IServiceCollection services, string baseAddress)
    {
        services.AddTransient<AntiforgeryHandler>();

        services.AddHttpClient(ClientName, client => client.BaseAddress = new Uri(baseAddress))
            .AddHttpMessageHandler<AntiforgeryHandler>();

        services.AddTransient(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName));
    }
}
