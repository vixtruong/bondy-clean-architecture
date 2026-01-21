namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class DataAnalyticsScopes
{
    public const string DataRead = "data.read";
    public const string DataManage = "data.manage";
    public const string AnalyticsRead = "analytics.read";
    public const string SystemMaintenance = "system.maintenance";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        DataRead, DataManage, AnalyticsRead, SystemMaintenance
    };
}