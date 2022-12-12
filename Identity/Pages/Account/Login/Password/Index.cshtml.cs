using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Identity.Exceptions;
using Identity.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Common;

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Identity.Pages.Login.Password;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private AuthorizationRequest _context;

    public IndexModel(
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction)
    {
        _signInManager = signInManager;
        _interaction = interaction;
    }

    [Required]
    [BindProperty]
    [DataType(DataType.Password)]
    [PageRemote(
        AdditionalFields = FieldNames.RequestVerificationToken,
        ErrorMessage = "Can`t sign in",
        HttpMethod = WebRequestMethods.Http.Post,
        PageHandler = "ValidatePassword")]
    public string Password { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; init; }

    public string SubmitButtonId => "submit";
    public Uri BackUrl => new("/Account/Login" + Request.QueryString.Value, UriKind.Relative);

    private string Username
    {
        get => HttpContext.Session.GetString(SessionKeys.Username);
        set => HttpContext.Session.SetString(SessionKeys.Username, value);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        _context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);

        var redirectUrl = new Uri("/Account/Login/Password?handler=SessionTimeout", UriKind.Relative);
        Response.Headers.Add(HeaderNames.Refresh, $"{Session.Timeout.TotalSeconds};url={redirectUrl}");

        if (string.IsNullOrEmpty(Username))
        {
            return await OnGetSessionTimeoutAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnGetSessionTimeoutAsync()
    {
        var redirectUrl = new Uri(Urls.Web, "AuthorizeRedirect").ToString();

        HttpContext.Session.Clear();

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

    public async Task<JsonResult> OnPostValidatePasswordAsync()
    {
        var result = await _signInManager.PasswordSignInAsync(Username, Password, false, lockoutOnFailure: true);

        return new JsonResult(result.Succeeded);
    }

    public IActionResult OnGetSuccess()
    {
        if (_context is not null)
        {
            if (_context.IsNativeClient())
            {
                return this.LoadingPage(ReturnUrl);
            }

            return Redirect(ReturnUrl);
        }

        if (Url.IsLocalUrl(ReturnUrl))
        {
            return Redirect(ReturnUrl);
        }
        else if (string.IsNullOrEmpty(ReturnUrl))
        {
            return Redirect("~/");
        }
        else
        {
            // user might have clicked on a malicious link - should be logged
            throw new InvalidUrlException("invalid return URL", nameof(ReturnUrl));
        }
    }
}
