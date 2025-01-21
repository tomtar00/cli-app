using System.Text;
using Backend.Exceptions;

namespace Backend.Files;

public static class FormatInspector
{
    public static FileContent.Type DetermineFileType(string? contentType, MemoryStream memoryStream)
    {
        if (!string.IsNullOrEmpty(contentType))
        {
            return contentType switch
            {
                "application/json" => FileContent.Type.Json,
                "text/csv" => FileContent.Type.CSV,
                "application/zip" => FileContent.Type.ZipArchive,
                _ => InspectContent(memoryStream)
            };
        }
        else
        {
            return InspectContent(memoryStream);
        }
    }

    public static FileContent.Type InspectContent(MemoryStream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);

        using StreamReader reader = new(stream, Encoding.UTF8, true, 1024, true);
        string? firstLine = reader.ReadLine();

        if (firstLine != null)
        {
            if (firstLine.TrimStart().StartsWith('{') || firstLine.TrimStart().StartsWith('['))
            {
                return FileContent.Type.Json;
            }
            else if (firstLine.Contains(','))
            {
                return FileContent.Type.CSV;
            }
            else if (IsZipFormat(stream))
            {
                return FileContent.Type.ZipArchive;
            }
            else
            {
                throw new FileProcessException("Failed to recoginze file format by it's content");
            }
        }
        else
        {
            throw new FileProcessException("Cannot recognize file format of an empty file");
        }
    }

    public static bool IsZipFormat(MemoryStream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        byte[] zipSignature = new byte[2];
        stream.Read(zipSignature, 0, 2);
        return zipSignature[0] == 'P' && zipSignature[1] == 'K';
    }
}