using Duende.IdentityServer.Services;

using Identity.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using Shared;

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Identity.Pages.Login;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityServerInteractionService _interaction;

    public Index(
        UserManager<ApplicationUser> userManager,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityServerInteractionService interaction,
        IStringLocalizer<Index> text)
    {
        _userManager = userManager;
        _schemeProvider = schemeProvider;
        _interaction = interaction;

        Text = text;
    }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; init; }

    [Required]
    [BindProperty]
    [Display(Name = nameof(Username))]
    [PageRemote(
        AdditionalFields = "__RequestVerificationToken",
        ErrorMessage = "Username doesn`t exist",
        HttpMethod = WebRequestMethods.Http.Post,
        PageHandler = "ValidateUsername")]
    public string Username
    {
        get => HttpContext.Session.GetString(SessionKeys.Username);
        set => HttpContext.Session.SetString(SessionKeys.Username, value);
    }

    public string SubmitButtonId => "submit";
    public IStringLocalizer<Index> Text { get; private init; }
    
    public async Task<IActionResult> OnGet()
    {
        var context = await _interaction.GetAuthorizationContextAsync(ReturnUrl);
        
        if (context?.IdP is not null && await _schemeProvider.GetSchemeAsync(context.IdP) is not null)
        {
            Username = context?.LoginHint;
        }

        return Page();
    }

    public async Task<JsonResult> OnPostValidateUsernameAsync()
    {
        var trimmedUsername = Username.Trim();

        var user = await _userManager.FindByNameAsync(trimmedUsername);
        var valid = user is not null;

        if (valid)
        {
            Username = trimmedUsername;
        }

        return new JsonResult(valid);
    }
}
