namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class PaymentsScopes
{
    public const string Create = "payments.create";
    public const string Read = "payments.read";
    public const string Refund = "payments.refund";
    public const string SubscriptionsCreate = "payments.subscriptions.create";
    public const string SubscriptionsRead = "payments.subscriptions.read";
    public const string SubscriptionsCancel = "payments.subscriptions.cancel";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Create, Read, Refund,
        SubscriptionsCreate, SubscriptionsRead, SubscriptionsCancel
    };
}