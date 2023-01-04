using Microsoft.Net.Http.Headers;

using System.Net;

namespace Identity.Extensions;

public static class PageModelExtension
{
    /// <summary>
    /// Renders a loading page that is used 
    /// to redirect back to the <paramref name="redirectUri"/>.
    /// </summary>
    public static IActionResult LoadingPage(this PageModel page, string redirectUri)
    {
        page.HttpContext.Response.StatusCode = (int) HttpStatusCode.OK;
        page.HttpContext.Response.Headers[HeaderNames.Location] = string.Empty;

        return page.RedirectToPage("/Redirect", new { RedirectUri = redirectUri });
    }
}
