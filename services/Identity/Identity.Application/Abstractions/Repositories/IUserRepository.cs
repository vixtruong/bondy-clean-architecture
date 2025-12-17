using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string emailNormalized, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}