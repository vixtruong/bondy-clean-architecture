
namespace Identity.Application.Abstractions.Security
{
    public interface IHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
