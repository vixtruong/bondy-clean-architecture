namespace Identity.Application.Results.Auth;

public record TokenResponse(string Access_Token, string Token_Type, int Expires_In, string Refresh_Token, string Scope);

public class DiscordUser
{
    public string Id { get; init; }
    public string Username { get; init; }
    public string Discriminator { get; init; }
    public string Email { get; init; }
    public string? Avatar { get; init; } = default!;
    public string AvatarUrl => GetDiscordAvatarUrl();

    public DiscordUser(string id, string username, string discriminator, string email, string? avatar)
    {
        Id = id;
        Username = username;
        Discriminator = discriminator;
        Email = email;
        Avatar = avatar;
    }

    private string GetDiscordAvatarUrl()
    {
        if (string.IsNullOrEmpty(Avatar))
        {
            if (int.TryParse(Discriminator, out var disc))
            {
                int index = disc % 5;
                return $"https://cdn.discordapp.com/embed/avatars/{index}.png";
            }
            return $"https://cdn.discordapp.com/embed/avatars/0.png";
        }

        if (Avatar.StartsWith("a_"))
            return $"https://cdn.discordapp.com/avatars/{Id}/{Avatar}.gif";

        return $"https://cdn.discordapp.com/avatars/{Id}/{Avatar}.png";
    }
}