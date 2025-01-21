namespace Backend.Exceptions;

public class FileProcessException : Exception
{
    public FileProcessException()
    {
    }

    public FileProcessException(string message) : base(message)
    {
    }
}