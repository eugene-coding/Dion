namespace Identity.Exceptions;

internal sealed class FindUserByClaimsFailedException : Exception
{
    public FindUserByClaimsFailedException(string message) : base(message)
    {
    }
}
