namespace locktopus_domain.Exceptions;

public class LoginItemNotFoundException : Exception
{
    public LoginItemNotFoundException()
    {
    }

    public LoginItemNotFoundException(string message)
        : base(message)
    {
    }

    public LoginItemNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}