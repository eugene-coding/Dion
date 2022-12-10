using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Identity.Pages.Account;

public sealed class AccessDeniedModel : PageModel
{
    public AccessDeniedModel(IStringLocalizer<AccessDeniedModel> localizer)
    {
        Localizer = localizer;
    }

    public IStringLocalizer<AccessDeniedModel> Localizer { get; private init; }
}
