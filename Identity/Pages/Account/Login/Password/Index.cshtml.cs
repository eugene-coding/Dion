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

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<IndexModel> _logger;
    private AuthorizationRequest _context;

    /// <param name="signInManager">The <see cref="SignInManager{TUser}"/>.</param>
    /// <param name="interaction">The <see cref="IIdentityServerInteractionService"/>.</param>
    public IndexModel(
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction,
        ILogger<IndexModel> logger)
    {
        _signInManager = signInManager;
        _interaction = interaction;
        _logger = logger;
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

        _logger.LogDebug($"68 {ReturnUrl}");
        _context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);

        _logger.LogDebug($"71 {ReturnUrl}");
        if (string.IsNullOrEmpty(Username))
        {
            _logger.LogDebug($"74 {ReturnUrl}");
            return await OnGetSessionTimeoutAsync();
        }

        _logger.LogDebug($"78 {ReturnUrl}");
        SetRefreshHeader();

        _logger.LogDebug($"81 {ReturnUrl}");
        BackUrl = new("/Account/Login" + Request.QueryString.Value, UriKind.Relative);

        _logger.LogDebug($"84 {ReturnUrl}");
        return Page();
    }

    private async Task<IActionResult> OnGetSessionTimeoutAsync()
    {
        _logger.LogDebug($"90 {ReturnUrl}");
        var redirectUrl = new Uri(Urls.Web, "AuthorizeRedirect").AbsoluteUri;

        _logger.LogDebug($"93 {ReturnUrl}");
        if (_context is not null)
        {
            _logger.LogDebug($"96 {ReturnUrl}");
            await _interaction.DenyAuthorizationAsync(_context, AuthorizationError.AccessDenied);

            _logger.LogDebug($"99 {ReturnUrl}");
            if (_context.IsNativeClient())
            {
                _logger.LogDebug($"102 {ReturnUrl}");
                return this.LoadingPage(redirectUrl);
            }
        }

        _logger.LogDebug($"107 {ReturnUrl}");
        return Redirect(redirectUrl);
    }

    /// <summary>
    /// Attempts to sign in the entered <see cref="Password">password</see> 
    /// and <see cref="Username">username</see>.
    /// </summary>
    /// <returns>
    /// Returns the <see cref="Task"/> containing the <see cref="JsonResult"/> 
    /// with <see langword="true"/> if the attempt is successfull, 
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public async Task<IActionResult> OnPostTryToSignInAsync()
    {
        _logger.LogDebug($"122 {ReturnUrl}");
        var result = await _signInManager.PasswordSignInAsync(
            Username, Password, false, false);

        _logger.LogDebug($"126 {ReturnUrl}");
        if (!result.Succeeded)
        {
            _logger.LogDebug($"129 {ReturnUrl}");
            return new JsonResult(result.Succeeded);
        }

        _logger.LogDebug($"133 {ReturnUrl}");
        return OnGetSuccess();
    }

    private IActionResult OnGetSuccess()
    {
        _logger.LogDebug($"151 {ReturnUrl}");
        if (_context is not null && _context.IsNativeClient())
        {
            _logger.LogDebug($"154 {ReturnUrl}");
            return this.LoadingPage(ReturnUrl);
        }

        _logger.LogDebug($"158 {ReturnUrl}");
        if (_context is not null || Url.IsLocalUrl(ReturnUrl))
        {
            _logger.LogDebug($"161 {ReturnUrl}");
            return Redirect(ReturnUrl);
        }

        _logger.LogDebug($"165 {ReturnUrl}");
        if (string.IsNullOrEmpty(ReturnUrl))
        {
            _logger.LogDebug($"168 {ReturnUrl}");
            return Redirect("~/");
        }

        _logger.LogDebug($"172 {ReturnUrl}");
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
