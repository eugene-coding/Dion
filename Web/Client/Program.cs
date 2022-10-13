using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using UI;

using Web.Client;

const string httpClientName = "backend";

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var rootComponents = builder.RootComponents;
var services = builder.Services;

rootComponents.Add<App>("#app");
rootComponents.Add<HeadOutlet>("head::after");

services.AddTransient<AntiforgeryHandler>();

services.AddHttpClient(httpClientName, client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
    .AddHttpMessageHandler<AntiforgeryHandler>();

services.AddTransient(sp => 
    sp.GetRequiredService<IHttpClientFactory>()
      .CreateClient(httpClientName));

services.AddAuthorizationCore();
services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();

await builder.Build().RunAsync();
