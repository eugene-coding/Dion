using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Identity.Exceptions;
using Identity.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Shared;

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Identity.Pages.Login.Password;

[SecurityHeaders]
[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEventService _events;
    private readonly IIdentityServerInteractionService _interaction;
    private AuthorizationRequest _context;

    public IndexModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IEventService events,
        IIdentityServerInteractionService interaction)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _events = events;
        _interaction = interaction;
    }

    [Required]
    [BindProperty]
    [DataType(DataType.Password)]
    [PageRemote(
        AdditionalFields = "__RequestVerificationToken",
        ErrorMessage = "Can`t sign in",
        HttpMethod = WebRequestMethods.Http.Post,
        PageHandler = "ValidatePassword")]
    public string Password { get; set; }
 
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; init; }

    private string Username
    {
        get => HttpContext.Session.GetString(SessionKeys.Username);
        set => HttpContext.Session.SetString(SessionKeys.Username, value);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        _context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);

        Response.Headers.Add("Refresh", $"{Shared.Config.SessionTimeout.TotalSeconds};url=/Account/Login/Password?handler=SessionTimeout");

        if (string.IsNullOrEmpty(Username))
        {
            return await OnGetSessionTimeoutAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnGetSessionTimeoutAsync()
    {
        HttpContext.Session.Clear();

        if (_context is not null)
        {
            await _interaction.DenyAuthorizationAsync(_context, AuthorizationError.AccessDenied);

            if (_context.IsNativeClient())
            {
                return this.LoadingPage(ReturnUrl);
            }

            return Redirect(ReturnUrl);
        }

        return Redirect($"{Shared.Config.WebUrl}/AuthorizeRedirect");
    }

    public async Task<JsonResult> OnPostValidatePasswordAsync()
    {
        var result = await _signInManager.PasswordSignInAsync(Username, Password, false, lockoutOnFailure: true);

        return new JsonResult(result.Succeeded);
    }

    public async Task<IActionResult> OnGetSuccess()
    {
        var user = await _userManager.FindByNameAsync(Username);
        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: _context?.Client.ClientId));

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

    public async Task<IActionResult> OnGetFailure()
    {
        var context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);
        await _events.RaiseAsync(new UserLoginFailureEvent(Username, "invalid credentials", clientId: context?.Client.ClientId));

        return Redirect("https://localhost:5002");
    }
}
