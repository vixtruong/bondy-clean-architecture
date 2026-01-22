using ContractsPurpose = Bondy.SharedKernel.Application.Commands.EmailPurpose;
using DomainPurpose = Mail.Domain.Enums.EmailPurpose;

namespace Mail.Application.Mapper;

public static class EmailPurposeMapper
{
    public static DomainPurpose ToDomain(this ContractsPurpose p) => p switch
    {
        ContractsPurpose.Welcome => DomainPurpose.Welcome,
        ContractsPurpose.OAuth2Welcome => DomainPurpose.OAuth2Welcome,
        ContractsPurpose.Registration => DomainPurpose.Registration,
        ContractsPurpose.ResetPassword => DomainPurpose.ResetPassword,
        _ => throw new ArgumentOutOfRangeException(nameof(p), p, "Unsupported email purpose")
    };
}
