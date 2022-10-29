namespace Identity.Exceptions;

internal sealed class ExternalAuthenticationException : Exception
{
    public ExternalAuthenticationException(string message) : base(message)
    {
    }
}
