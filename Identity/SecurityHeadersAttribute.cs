using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;

namespace Identity;

public sealed class SecurityHeadersAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var result = context.Result;

        if (result is PageResult)
        {
            var headers = context.HttpContext.Response.Headers;

            if (!headers.ContainsKey(HeaderNames.XContentTypeOptions))
            {
                headers.Add(HeaderNames.XContentTypeOptions, "nosniff");
            }

            if (!headers.ContainsKey(HeaderNames.XFrameOptions))
            {
                headers.Add(HeaderNames.XFrameOptions, "SAMEORIGIN");
            }

            var csp = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";
            // also consider adding upgrade-insecure-requests once you have HTTPS in place for production
            //csp += "upgrade-insecure-requests;";
            // also an example if you need client images to be displayed from twitter
            // csp += "img-src 'self' https://pbs.twimg.com;";

            if (!headers.ContainsKey(HeaderNames.ContentSecurityPolicy))
            {
                headers.Add(HeaderNames.ContentSecurityPolicy, csp);
            }

            if (!headers.ContainsKey(Common.HeaderNames.XContentSecurityPolicy))
            {
                headers.Add(Common.HeaderNames.XContentSecurityPolicy, csp);
            }

            if (!headers.ContainsKey(Common.HeaderNames.ReferrerPolicy))
            {
                headers.Add(Common.HeaderNames.ReferrerPolicy, "no-referrer");
            }
        }
    }
}
