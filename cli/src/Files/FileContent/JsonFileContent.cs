using Backend.Exceptions;
using System.Text.Json;

namespace Backend.Files;

public class JsonFileContent(byte[] content) : FileContent(content)
{
    public override TContent Deserialize<TContent>()
    {
        string jsonString = System.Text.Encoding.UTF8.GetString(_content);
        return JsonSerializer.Deserialize<TContent>(jsonString) ??
            throw new FileProcessException("Failed to deserialize JSON");
    }
}