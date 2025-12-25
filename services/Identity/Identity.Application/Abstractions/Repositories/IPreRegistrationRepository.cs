using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Abstractions.Repositories;
public interface IPreRegistrationRepository
{
    Task<PreRegistration> AddAsync(PreRegistration pre);

    Task<PreRegistration?> GetByEmailAsync(Email email);

    Task RemoveAsync(PreRegistration pre);
}
