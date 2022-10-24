using Microsoft.AspNetCore.Authentication;

namespace Identity.Pages.Login;

public sealed class ExternalProvider
{
    public string DisplayName { get; set; }
    public string AuthenticationScheme { get; set; }
}
