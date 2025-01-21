using Backend.Exceptions;

namespace Backend.Files;

public static class FileDownloader
{
    public static async Task<FileContent> Download(string url)
    {
        string? contentType;
        using MemoryStream memoryStream = new();

        try
        {
            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(url);
            using Stream contentStream = await response.Content.ReadAsStreamAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new FileGetException("Getting file failed with status code: " + response.StatusCode);
            }

            contentType = response.Content.Headers.ContentType?.MediaType;

            await contentStream.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
        }
        catch (Exception ex)
        {
            throw new FileGetException(ex.Message);
        }

        return FileFactory.CreateFile(contentType, memoryStream);
    }
}