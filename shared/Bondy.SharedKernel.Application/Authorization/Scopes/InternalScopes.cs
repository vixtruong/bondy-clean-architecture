namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class InternalScopes
{
    public const string All = "internal.*";
    public const string WebhookReceive = "webhook.receive";
    public const string WebhookManage = "webhook.manage";
    public const string PartnerIntegration = "partner.integration";
    public const string ApiGatewayHealth = "apigateway.health";

    public static readonly IReadOnlyCollection<string> AllScopes = new[]
    {
        All, WebhookReceive, WebhookManage, PartnerIntegration, ApiGatewayHealth
    };
}