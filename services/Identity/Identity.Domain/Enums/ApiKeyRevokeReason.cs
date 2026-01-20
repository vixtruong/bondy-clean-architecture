namespace Identity.Domain.Enums;

public enum ApiKeyRevokeReason
{
    UserAction,
    RotationCleanup,
    SecurityIncident,
    Expired
}

