using Duende.IdentityServer.Services;

using Identity.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Identity.Pages.Login;

/// <summary>The <see cref="PageModel">model</see> for the login page.</summary>
[SecurityHeaders]
[AllowAnonymous]
public sealed class IndexModel : PageModel
{
    /// <summary>ID of the form submit button.</summary>
    public const string SubmitButtonId = "submit";

    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityServerInteractionService _interaction;

    /// <summary>Creates the <see cref="IndexModel"/> instance.</summary>
    /// <param name="schemeProvider">The <see cref="IAuthenticationSchemeProvider"/>.</param>
    /// <param name="interaction">The <see cref="IIdentityServerInteractionService"/>.</param>
    /// <param name="localizer">The <see cref="IStringLocalizer"/>.</param>
    public IndexModel(
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityServerInteractionService interaction,
        IStringLocalizer<IndexModel> localizer)
    {
        _schemeProvider = schemeProvider;
        _interaction = interaction;
        Localizer = localizer;
    }

    /// <summary>Gets or initializes the return URL.</summary>
    /// <value>
    /// The URL to which the user will be redirected 
    /// after the authorization process is completed.
    /// </value>
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; init; }

    /// <summary>Gets or sets the username.</summary>
    /// <remarks>The username stored in the session. When setting, the value is trimmed.</remarks>
    [Required(ErrorMessage = "Enter the username")]
    [BindProperty]
    [Display(Name = "Username", Prompt = "Username")]
    [PageRemote(
        AdditionalFields = FieldNames.RequestVerificationToken,
        ErrorMessage = "User doesn't exist",
        HttpMethod = WebRequestMethods.Http.Post,
        PageHandler = "CheckUsername")]
    public string Username
    {
        get => HttpContext.Session.GetString(SessionKeys.Username);
        set => HttpContext.Session.SetString(SessionKeys.Username, value.Trim());
    }

    /// <inheritdoc cref="IStringLocalizer"/>
    public IStringLocalizer<IndexModel> Localizer { get; }

    /// <summary>Executed on <c>GET</c> request.</summary>
    /// <remarks>
    /// Sets the <see cref="Duende.IdentityServer.Models.AuthorizationRequest.LoginHint">login hint</see>, 
    /// if it exists, as the <see cref="Username">username</see> and loads the page.
    /// </remarks>
    /// <returns>Returns the <see cref="Task"/> that loads the page.</returns>
    public async Task OnGetAsync()
    {
        var hint = await GetLoginHint();

        if (hint is not null)
        {
            Username = hint;
        }
    }

    /// <summary>Checks if there is an entry with the username entered.</summary>
    /// <param name="userManager">The <see cref="UserManager{TUser}"/>.</param>
    /// <returns>
    /// Returns the <see cref="Task"/> containing the <see cref="JsonResult"/> 
    /// with <see langword="true"/> if a record with the entered username is found, 
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public async Task<JsonResult> OnPostCheckUsernameAsync(
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByNameAsync(Username);

        return new JsonResult(user is not null);
    }

    /// <summary>Executed when the form is successfully validated.</summary>
    /// <remarks>Redirects to the password entry page.</remarks>
    /// <returns>The <see cref="RedirectToPageResult"/>.</returns>
    public RedirectToPageResult OnGetSuccess()
    {
        return RedirectToPage("/Account/Login/Password/Index", new { ReturnUrl });
    }

    private async Task<string> GetLoginHint()
    {
        var context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);

        if (context?.IdP is not null && await _schemeProvider.GetSchemeAsync(context.IdP) is not null)
        {
            return context.LoginHint;
        }

        return null;
    }
}
