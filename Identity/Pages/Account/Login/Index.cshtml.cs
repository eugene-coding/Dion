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

[SecurityHeaders]
[AllowAnonymous]
public sealed class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityServerInteractionService _interaction;

    public Index(
        UserManager<ApplicationUser> userManager,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityServerInteractionService interaction,
        IStringLocalizer<Index> localizer)
    {
        _userManager = userManager;
        _schemeProvider = schemeProvider;
        _interaction = interaction;

        Localizer = localizer;
    }

    /// <summary>Gets or initializes the return URL.</summary>
    /// <value>
    /// The URL to which the user will be redirected 
    /// after the authorization process is completed
    /// </value>
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; init; }

    /// <summary>Gets or sets the username.</summary>
    /// <remarks>
    /// The username stored in the session.
    /// When setting, the value is trimmed.
    /// </remarks>
    [Required]
    [BindProperty]
    [Display(Name = nameof(Username))]
    [PageRemote(
        AdditionalFields = FieldNames.RequestVerificationToken,
        ErrorMessage = "Username doesn`t exist",
        HttpMethod = WebRequestMethods.Http.Post,
        PageHandler = "ValidateUsername")]
    public string Username
    {
        get => HttpContext.Session.GetString(SessionKeys.Username);
        set => HttpContext.Session.SetString(SessionKeys.Username, value.Trim());
    }

    /// <inheritdoc cref="IStringLocalizer" />
    public IStringLocalizer<Index> Localizer { get; private init; }

    public async Task OnGetAsync()
    {
        var hint = await GetLoginHint();

        if (hint is not null)
        {
           Username = hint;
        }
    }

    /// <summary>Checks if there is an entry with the username entered.</summary>
    /// <returns>
    /// Returns the <see cref="Task"/> that contains <see cref="JsonResult"/> 
    /// with <see langword="true"/> if a record with the entered username is found, 
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public async Task<JsonResult> OnPostValidateUsernameAsync()
    {
        var user = await _userManager.FindByNameAsync(Username);

        return new JsonResult(user is not null);
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
