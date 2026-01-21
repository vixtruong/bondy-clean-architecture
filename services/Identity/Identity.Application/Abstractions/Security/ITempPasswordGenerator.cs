using Identity.Domain.Entities;


namespace Identity.Application.Abstractions.Security;

public interface ITempPasswordGenerator
{
    string Generate();
}
