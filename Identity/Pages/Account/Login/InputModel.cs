using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Identity.Pages.Login;

public class InputModel
{
    [Required]
    [Display(Name = nameof(Username))]
    [PageRemote(ErrorMessage = "Username doesn`t exist", HttpMethod = WebRequestMethods.Http.Post, PageHandler = "ValidateUsername")]
    public string Username { get; set; }

    public string ReturnUrl { get; set; }
}
