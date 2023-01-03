using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Identity.Pages.Account.Register;

public sealed class InputModel
{
    [Required(ErrorMessage = "Enter the email")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Email")]
    [PageRemote(
        ErrorMessage = "The email already exists",
        HttpMethod = WebRequestMethods.Http.Post,
        PageHandler = "CheckEmail")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}
