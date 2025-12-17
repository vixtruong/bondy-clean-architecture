namespace Identity.Domain.Enums;

public enum OtpSubjectType
{
    PreRegistration = 0,
    User = 1
}

public enum OtpPurpose
{
    VerifyEmail = 0,
    ResetPassword = 1,
    Login = 2
}