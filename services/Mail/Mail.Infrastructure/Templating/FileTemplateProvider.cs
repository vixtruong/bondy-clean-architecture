using Mail.Application.Abstractions.Templating;

namespace Mail.Infrastructure.Templating;

public sealed class FileTemplateProvider : ITemplateProvider
{
    private readonly string _basePath;

    public FileTemplateProvider(string basePath) => _basePath = basePath;

    public Task<string> GetAsync(string fileName)
        => File.ReadAllTextAsync(Path.Combine(_basePath, fileName));
}
