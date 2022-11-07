using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Identity.Pages.Account;

public class AccessDeniedModel : PageModel
{
    public AccessDeniedModel(IStringLocalizer<AccessDeniedModel> text)
    {
        Text = text;
    }

    public IStringLocalizer<AccessDeniedModel> Text { get; private init; }

    public void OnGet()
    {
    }
}
