using Backend.Exceptions;

namespace Backend.Files;

public static class FileFactory
{
    public static FileContent CreateFile(FileContent.Type fileType, MemoryStream content)
    {
        byte[] buffer = content.GetBuffer();
        return fileType switch
        {
            FileContent.Type.Json => new JsonFileContent(buffer),
            FileContent.Type.CSV => new CsvFileContent(buffer),
            FileContent.Type.ZipArchive => new ZipFileContent(buffer),
            _ => throw new FileProcessException("Cannot create file with format: " + fileType)
        };
    }
    public static FileContent CreateFile(string? contentType, MemoryStream content)
    {
        FileContent.Type fileType = FormatInspector.DetermineFileType(contentType, content);
        return CreateFile(fileType, content);
    }
    public static FileContent CreateFile(MemoryStream content)
    {
        FileContent.Type fileType = FormatInspector.InspectContent(content);
        return CreateFile(fileType, content);
    }
}