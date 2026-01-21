namespace Bondy.SharedKernel.Application.Authorization.Scopes;

public static class UploadScopes
{
    public const string Image = "upload.image";
    public const string Video = "upload.video";
    public const string File = "upload.file";
    public const string Delete = "upload.delete";
    public const string Manage = "upload.manage";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        Image, Video, File, Delete, Manage
    };
}