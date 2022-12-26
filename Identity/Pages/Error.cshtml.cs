using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Identity.Pages;

/// <summary><see cref="PageModel">Model</see> for the error page.</summary>
[AllowAnonymous]
[SecurityHeaders]
public class ErrorModel : PageModel
{
    /// <summary>Creates the <see cref="ErrorModel"/> instance.</summary>
    /// <param name="localizer">The <see cref="IStringLocalizer{T}"/>.</param>
    public ErrorModel(IStringLocalizer<Index> localizer)
    {
        Localizer = localizer;
    }

    /// <inheritdoc cref="IStringLocalizer{T}"/>
    public IStringLocalizer<Index> Localizer { get; private init; }

    /// <inheritdoc cref="ErrorMessage"/>
    public ErrorMessage Error { get; private set; }

    /// <summary>Executed on <c>GET</c> request.</summary>
    /// <remarks>
    /// Sets an <see cref="Error">error message</see> 
    /// based on the <paramref name="errorId">error ID</paramref> if the context exists.
    /// </remarks>
    /// <param name="interaction">The <see cref="IIdentityServerInteractionService"/>.</param>
    /// <param name="environment">The <see cref="IWebHostEnvironment"/>.</param>
    /// <param name="errorId">Error ID obtained from the query string.</param>
    /// <returns>Return the <see cref="Task"/> that loads the page asynchronously.</returns>
    public async Task OnGetAsync(
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
