using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Identity.Pages;

[AllowAnonymous]
[SecurityHeaders]
public class ErrorModel : PageModel
{
    public ErrorModel(IStringLocalizer<Index> localizer)
    {
        Localizer = localizer;
    }

    public IStringLocalizer<Index> Localizer { get; private init; }
    public ErrorMessage Error { get; private set; }

    public async Task OnGet(
        [FromServices] IIdentityServerInteractionService interaction,
        [FromServices] IWebHostEnvironment environment,
        string errorId)
    {
        var message = await interaction.GetErrorContextAsync(errorId);

        if (message is not null)
        {
            Error = message;

            if (environment.IsDevelopment())
            {
                message.ErrorDescription = null;
            }
        }
    }
}
