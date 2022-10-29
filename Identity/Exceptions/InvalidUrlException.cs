namespace Identity.Exceptions;

internal sealed class InvalidUrlException : ArgumentException
{
    public InvalidUrlException(string message, string paramName) : base(message, paramName)
    {
    }
}
