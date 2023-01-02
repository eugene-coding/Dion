using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Identity.Pages.Account.Registration;

public sealed class InputModel
{
    [Required(ErrorMessage = "Enter the username")]
    [Display(Name = "Username", Prompt = "Username")]
    [PageRemote(
        ErrorMessage = "The username already exists",
        HttpMethod = WebRequestMethods.Http.Post,
        PageHandler = "CheckUsername")]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}
