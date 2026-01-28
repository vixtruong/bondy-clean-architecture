using Shared.Storage.Abstractions;

namespace Shared.Storage.Local;

public class LocalFileStorage : IFileStorage
{
    private const string UploadRoot = "uploads";

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required", nameof(fileName));

        fileName = fileName.Replace("\\", "/").TrimStart('/');

        var fullPath = Path.Combine(UploadRoot, fileName);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directory) &&
            !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var output = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await fileStream.CopyToAsync(output);

        return "/" + fullPath.Replace("\\", "/");
    }
}

