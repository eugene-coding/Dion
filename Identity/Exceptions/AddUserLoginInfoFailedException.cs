namespace Identity.Exceptions;

internal sealed class AddUserLoginInfoFailedException : Exception
{
    public AddUserLoginInfoFailedException(string message) : base(message)
    {
    }
}
