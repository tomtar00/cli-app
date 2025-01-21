namespace Backend.Exceptions;

public class FileGetException : Exception
{
    public FileGetException()
    {
    }

    public FileGetException(string message) : base(message)
    {
    }
}
