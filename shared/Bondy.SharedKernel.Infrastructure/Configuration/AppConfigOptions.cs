
namespace Bondy.SharedKernel.Infrastructure.Configuration;

public sealed class AppConfigOptions
{
    public const string SectionName = "App";

    public string Environment { get; init; } = default!;
}