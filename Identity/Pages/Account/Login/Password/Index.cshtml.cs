using Common;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Identity.Exceptions;
using Identity.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;

using System.Net;

namespace Identity.Pages.Login.Password;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    /// <inheritdoc cref="Login.IndexModel.SubmitButtonId"/>
    public const string SubmitButtonId = "submit";

    private readonly IIdentityServerInteractionService _interaction;
    private AuthorizationRequest _context;

    public IndexModel(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    /// <summary>Gets the username.</summary>
    /// <remarks>The username stored in the session.</remarks>
    public string Username => HttpContext.Session.GetString(SessionKeys.Username);

    /// <summary>Gets or sets the user password.</summary>
    [Required, FromForm]
    [DataType(DataType.Password)]
    [PageRemote(
        AdditionalFields = FieldNames.RequestVerificationToken,
        ErrorMessage = "Can`t sign in",
        HttpMethod = WebRequestMethods.Http.Post,
        PageHandler = "TryToSignIn")]
    public string Password { get; set; }

    /// <inheritdoc cref="Login.IndexModel.ReturnUrl"/>
    [BindProperty]
    public string ReturnUrl { get; set; }

    public Uri BackUrl { get; private set; }

    public async Task<IActionResult> OnGetAsync(string returnUrl)
    {
        ReturnUrl = returnUrl;

        _context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);

        if (string.IsNullOrEmpty(Username))
        {
            return await OnGetSessionTimeoutAsync();
        }

        SetRefreshHeader();

        BackUrl = new("/Account/Login" + Request.QueryString.Value, UriKind.Relative);

        return Page();
    }

    private async Task<IActionResult> OnGetSessionTimeoutAsync()
    {
        var redirectUrl = new Uri(Urls.Web, "AuthorizeRedirect").AbsoluteUri;

        if (_context is not null)
        {
            await _interaction.DenyAuthorizationAsync(_context, AuthorizationError.AccessDenied);

            if (_context.IsNativeClient())
            {
                return this.LoadingPage(redirectUrl);
            }
        }

        return Redirect(redirectUrl);
    }

    /// <summary>
    /// Attempts to sign in the entered <see cref="Password">password</see> 
    /// and <see cref="Username">username</see>.
    /// </summary>
    /// <param name="signInManager">The <see cref="SignInManager{TUser}"/> from the DI container.</param>
    /// <returns>
    /// Returns the <see cref="Task"/> containing the <see cref="JsonResult"/> 
    /// with <see langword="true"/> if the attempt is successfull, 
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public async Task<JsonResult> OnPostTryToSignInAsync([FromServices] SignInManager<ApplicationUser> signInManager)
    {
        var result = await signInManager.PasswordSignInAsync(
            Username, Password, false, false);

        return new JsonResult(result.Succeeded);
    }

    private IActionResult OnGetSuccess()
    {
        if (_context is not null && _context.IsNativeClient())
        {
            return this.LoadingPage(ReturnUrl);
        }

        if (_context is not null || Url.IsLocalUrl(ReturnUrl))
        {
            return Redirect(ReturnUrl);
        }

        if (string.IsNullOrEmpty(ReturnUrl))
        {
            return Redirect("~/");
        }

        throw new InvalidUrlException("The return URL is invalid", nameof(ReturnUrl));
    }

    private void SetRefreshHeader()
    {
        var page = "/Account/Login/Password";
        var handler = "SessionTimeout";

        var redirectUrl = new Uri($"{page}?handler={handler}", UriKind.Relative);

        Response.Headers.Add(HeaderNames.Refresh, $"{Session.Timeout.TotalSeconds};url={redirectUrl}");
    }
}
