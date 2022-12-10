using Shared;

namespace Web.Client;

internal sealed class AntiforgeryHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add(HeaderNames.XCSRF, "1");

        return base.SendAsync(request, cancellationToken);
    }
}
