using System.IO.Compression;
using Backend.Exceptions;

namespace Backend.Files;

public class ZipFileContent(byte[] content) : FileContent(content)
{
    public override TContent Deserialize<TContent>()
    {
        using MemoryStream memoryStream = new(_content);
        using ZipArchive archive = new(memoryStream);

        // we expect only one file inside the archive
        if (archive.Entries.Count != 1)
        {
            throw new FileProcessException("Zip archive must contain exactly one file");
        }

        using Stream entryStream = archive.Entries[0].Open();
        using MemoryStream innerFileMemoryStream = new();
        entryStream.CopyTo(innerFileMemoryStream);
        innerFileMemoryStream.Seek(0, SeekOrigin.Begin);

        return FileFactory.CreateFile(innerFileMemoryStream).Deserialize<TContent>();
    }
}