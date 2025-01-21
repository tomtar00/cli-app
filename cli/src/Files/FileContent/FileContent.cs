namespace Backend.Files;

// this class represents file content
// in it's current form content is stored as byte array
// but it would be preferable to store it as stream
// to avoid memory issues with large files
public abstract class FileContent(byte[] content)
{
    protected byte[] _content = content;

    public abstract TContent Deserialize<TContent>() where TContent : new();

    public enum Type
    {
        Json, CSV, ZipArchive
    }
}