namespace Backend.Exceptions;

public class CommandParseException : Exception
{
    public CommandParseException()
    {
    }

    public CommandParseException(string message) : base(message)
    {
    }

    public CommandParseException(string message, Exception innerException) : base(message, innerException)
    {

    }
}