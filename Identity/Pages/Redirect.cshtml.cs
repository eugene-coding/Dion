using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Identity.Pages;

[AllowAnonymous]
public class RedirectModel : PageModel
{
    public RedirectModel(IStringLocalizer<RedirectModel> localizer)
    {
        Localizer = localizer;
    }

    public IStringLocalizer<RedirectModel> Localizer { get; private init; }

    [BindProperty(SupportsGet = true)]
    public string RedirectUri { get; init; }

    public IActionResult OnGet()
    {
        if (!Url.IsLocalUrl(RedirectUri))
        {
            return RedirectToPage("/Error");
        }

        return Page();
    }
}
