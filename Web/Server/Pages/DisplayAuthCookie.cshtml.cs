using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Server.Pages;

public class DisplayAuthCookieModel : PageModel
{
    public IDictionary<string, string?> AuthenticateProperties = default!;
    
    public async Task OnGetAsync()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync();

        AuthenticateProperties = authenticateResult.Properties?.Items ?? new Dictionary<string, string?>();
    }
}
