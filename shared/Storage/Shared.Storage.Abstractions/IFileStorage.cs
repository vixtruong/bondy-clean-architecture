namespace Shared.Storage.Abstractions;
public interface IFileStorage
{
    Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType);
}