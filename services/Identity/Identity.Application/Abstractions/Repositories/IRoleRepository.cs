using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByCodeAsync(string code);

    Task<IReadOnlyCollection<Role>> GetByCodesAsync(
        IEnumerable<string> codes);

    Task AddAsync(Role role);

    Task UpdateAsync(Role role);
}