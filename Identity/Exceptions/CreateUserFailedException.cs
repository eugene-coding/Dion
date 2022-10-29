namespace Identity.Exceptions;

internal sealed class CreateUserFailedException : Exception
{
    public CreateUserFailedException(string message) : base(message)
    {
    }
}
