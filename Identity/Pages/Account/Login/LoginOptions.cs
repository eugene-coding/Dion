namespace Identity.Pages.Login;

public static class LoginOptions
{
    public const string InvalidCredentialsErrorMessage = "Invalid username or password";
    
    public static bool AllowLocalLogin => true;
    public static bool AllowRememberLogin => true;
    public static TimeSpan RememberMeLoginDuration => TimeSpan.FromDays(30);
}
