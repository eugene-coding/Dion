using Microsoft.AspNetCore.Authentication;

namespace Identity.Extensions;

public static class HttpContextExtension
{
    /// <summary>
    /// Determines if the authentication scheme support signout.
    /// </summary>
    public static async Task<bool> GetSchemeThatSupportsSignOutAsync(this HttpContext context, string scheme)
    {
        var provider = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
        var handler = await provider.GetHandlerAsync(context, scheme);
        
        return handler is IAuthenticationSignOutHandler;
    }
}
