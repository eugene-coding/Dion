using Duende.IdentityServer.Models;

using Identity.Exceptions;
using Identity.Models;

using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;

using System.Net;
using Identity.Extensions;

namespace Identity.Pages.Login;

/// <summary>The <see cref="PageModel">model</see> for the password page.</summary>
[SecurityHeaders]
[AllowAnonymous]
public class PasswordModel : PageModel
{
    /// <inheritdoc cref="Login.IndexModel.SubmitButtonId"/>
    public const string SubmitButtonId = "submit-button";

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private AuthorizationRequest _context;

    /// <summary>Creates the <see cref="PasswordModel"/> instance.</summary>
    /// <param name="signInManager">The <see cref="SignInManager{TUser}"/>.</param>
    /// <param name="interaction">The <see cref="IIdentityServerInteractionService"/>.</param>
    /// <param name="localizer">The <see cref="IStringLocalizer"/>.</param>
    public PasswordModel(
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction,
        IStringLocalizer<PasswordModel> localizer)
    {
        _signInManager = signInManager;
        _interaction = interaction;
        Localizer = localizer;
    }

    /// <inheritdoc cref="IStringLocalizer"/>
    public IStringLocalizer<PasswordModel> Localizer { get; }

    /// <summary>Gets the username.</summary>
    /// <remarks>The username stored in the session.</remarks>
    public string Username => HttpContext.Session.GetString(SessionKeys.Username);

    /// <summary>Gets or sets the user password.</summary>
    [Required(ErrorMessage = "Enter the password")]
    [BindProperty]
    [Display(Name = "Password", Prompt = "Password")]
    [DataType(DataType.Password)]
    [PageRemote(
        AdditionalFields = FieldNames.RequestVerificationToken,
        ErrorMessage = "Wrong password",
        HttpMethod = WebRequestMethods.Http.Post,
        PageHandler = "CheckPassword")]
    public string Password { get; set; }

    /// <inheritdoc cref="Login.IndexModel.ReturnUrl"/>
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; init; }

    /// <summary>Gets or sets the login page URL.</summary>
    /// <remarks>Link to the previous authorization step.</remarks>
    public string LoginUrl { get; private set; }

    /// <summary>Executed on <c>GET</c> request.</summary>
    /// <remarks>
    /// If there is no <see cref="Username">username</see>
    /// in the <see cref="HttpContext.Session">session</see>,
    /// the user will be redirected to the login page to enter it.
    /// </remarks>
    /// <returns>Returns the <see cref="Task"/> that loads the page.</returns>
    public async Task<IActionResult> OnGetAsync()
    {
        LoginUrl = Url.Page("Index", new { ReturnUrl });

        if (string.IsNullOrWhiteSpace(Username))
        {
            return Redirect(LoginUrl);
        }

        _context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);

        return Page();
    }

    /// <summary>
    /// Attempts to sign in the entered <see cref="Password">password</see> 
    /// and <see cref="Username">username</see>.
    /// </summary>
    /// <remarks>
    /// If there is no <see cref="Username">username</see>
    /// in the <see cref="HttpContext.Session">session</see>,
    /// the user will be redirected to the login page to enter it.
    /// </remarks>
    /// <param name="userManager">The <see cref="UserManager{TUser}"/>.</param>
    /// <returns>
    /// Returns the <see cref="Task"/> containing the <see cref="JsonResult"/> 
    /// with <see langword="true"/> if the attempt is successfull, 
    /// otherwise - <see langword="false"/>.
    /// </returns>
    public async Task<IActionResult> OnPostCheckPasswordAsync(
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            return Redirect(LoginUrl);
        }

        var user = await userManager.FindByNameAsync(Username);

        if (user is not null)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(
                user, Password, false);

            return new JsonResult(result.Succeeded);
        }

        return new JsonResult(false);
    }

    /// <summary>Executed when submitting the form.</summary>
    /// <remarks>
    /// <para>
    /// Signs in using the <see cref="Username">username</see> 
    /// and the <see cref="Password">password</see>
    /// and then redirects to the <see cref="ReturnUrl">return URL</see>.
    /// </para>
    /// <para>
    /// If there is no <see cref="Username">username</see>
    /// in the <see cref="HttpContext.Session">session</see>,
    /// the user will be redirected to the login page to enter it.
    /// </para>
    /// </remarks>
    /// <returns>
    /// Returns the <see cref="Task"/> containing the <see cref="IActionResult"/>.
    /// </returns>
    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            return Redirect(LoginUrl);
        }

        await _signInManager.PasswordSignInAsync(
            Username, Password, false, false);

        return RedirectToReturnUrl();
    }

    private IActionResult RedirectToReturnUrl()
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
}
