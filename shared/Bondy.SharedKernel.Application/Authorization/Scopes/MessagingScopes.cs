namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class MessagingScopes
{
    public const string MessagesRead = "messages.read";
    public const string MessagesSend = "messages.send";
    public const string MessagesDelete = "messages.delete";
    public const string AttachUpload = "messages.attach.upload";
    public const string AttachDownload = "messages.attach.download";
    public const string ConversationsStart = "conversations.start";
    public const string ConversationsManage = "conversations.manage";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        MessagesRead, MessagesSend, MessagesDelete,
        AttachUpload, AttachDownload,
        ConversationsStart, ConversationsManage
    };
}