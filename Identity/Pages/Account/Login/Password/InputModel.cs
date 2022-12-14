using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Identity.Pages.Login.Password;

/// <summary>Stores data obtained using model binding.</summary>
public sealed class InputModel
{
    /// <summary>Gets or sets the user password.</summary>
    [Required, FromForm]
    [BindRequired]
    [DataType(DataType.Password)]
    [PageRemote(
        ErrorMessage = "Can`t sign in",
        HttpMethod = WebRequestMethods.Http.Post,
        PageHandler = "TryToSignIn")]
    public string Password { get; set; }

    /// <inheritdoc cref="Login.IndexModel.ReturnUrl"/>
    [FromQuery]
    public string ReturnUrl { get; set; }
}
