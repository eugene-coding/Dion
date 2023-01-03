namespace Identity.Pages.Account;

public sealed class AccessDeniedModel : PageModel
{
    public AccessDeniedModel(IStringLocalizer<AccessDeniedModel> localizer)
    {
        Localizer = localizer;
    }

    public IStringLocalizer<AccessDeniedModel> Localizer { get; }
}
