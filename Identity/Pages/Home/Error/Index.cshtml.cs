using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Identity.Pages.Error;

[AllowAnonymous]
[SecurityHeaders]
public class Index : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IWebHostEnvironment _environment;

    public ViewModel View { get; set; }
    public IStringLocalizer<Index> Text { get; init; }

    public Index(
        IIdentityServerInteractionService interaction,
        IWebHostEnvironment environment,
        IStringLocalizer<Index> text)
    {
        _interaction = interaction;
        _environment = environment;
        Text = text;
    }

    public async Task OnGet(string errorId)
    {
        View = new ViewModel();

        // retrieve error details from identityserver
        var message = await _interaction.GetErrorContextAsync(errorId);

        if (message != null)
        {
            View.Error = message;

            if (!_environment.IsDevelopment())
            {
                // only show in development
                message.ErrorDescription = null;
            }
        }
    }
}
