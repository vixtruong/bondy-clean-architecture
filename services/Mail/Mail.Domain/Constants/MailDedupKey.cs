namespace Mail.Domain.Constants;

public static class MailDedupKey
{
    public static string Otp(string email, string purpose, string tokenId) => $"otp:{email}:{purpose}:{tokenId}";
    public static string Welcome(string userId) => $"welcome:{userId}";
}