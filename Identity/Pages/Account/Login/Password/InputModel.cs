using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Login.Password;

public class InputModel : Login.InputModel
{
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
