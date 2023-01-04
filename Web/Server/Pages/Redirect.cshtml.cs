using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Server.Pages;

/// <summary>The <see cref="PageModel"/> for the Redirect page.</summary>
/// <remarks>
/// <para>
/// The page is designed to check if the <see cref="PageModel.User">user</see> is authenticated. 
/// Since the page is located on the <see cref="Server">server</see> side, 
/// the check is carried out without loading libraries for the <see cref="Client">client</see> (WASM) part of the application.
/// </para>
/// <para>
/// If the <see cref="PageModel.User">user</see> is not <see cref="System.Security.Principal.IIdentity.IsAuthenticated">authenticated</see>, 
/// he will be automatically redirected to the login page thanks to the 
/// <see cref="AuthorizationEndpointConventionBuilderExtensions.RequireAuthorization{TBuilder}(TBuilder)">RequireAuthorization</see> 
/// convention in the <see cref="Program"/> file, 
/// otherwise - to the <see cref="UI.Pages.Index">main page</see> of the <see cref="Client">client</see> side.
/// </para>
/// </remarks>
public class RedirectModel : PageModel
{
    /// <summary>Executed on <c>GET</c> request.</summary>
    /// <remarks>Redirects to the <see cref="UI.Pages.Index">Index</see> page.</remarks>
    /// <returns>The <see cref="RedirectToPageResult"/>.</returns>
    public RedirectToPageResult OnGet()
    {
        return RedirectToPage("Index");
    }
}
