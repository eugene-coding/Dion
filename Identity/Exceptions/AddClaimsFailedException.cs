namespace Identity.Exceptions;

internal sealed class AddClaimsFailedException : Exception
{
    public AddClaimsFailedException(string message) : base(message)
    {
    }
}
