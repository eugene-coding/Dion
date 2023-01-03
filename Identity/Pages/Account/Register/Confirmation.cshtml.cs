namespace Identity.Pages.Account.Register;

[AllowAnonymous]
[SecurityHeaders]
public class ConfirmationModel : PageModel
{
    public ConfirmationModel(IStringLocalizer<ConfirmationModel> localizer)
    {
        Localizer = localizer;
    }

    public IStringLocalizer<ConfirmationModel> Localizer { get; }
}
