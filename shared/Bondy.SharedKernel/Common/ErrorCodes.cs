namespace Bondy.SharedKernel.Constants;

public static class ErrorCodes
{
    public static class Validation
    {
        public const string Argument = "validation.argument";
        public const string Required = "validation.required";
        public const string InvalidFormat = "validation.invalid_format";
        public const string Range = "validation.range";
    }

    public static class Auth
    {
        public const string Unauthorized = "auth.unauthorized";
        public const string Forbidden = "auth.forbidden";
        public const string InvalidCredentials = "auth.invalid_credentials";
        public const string UserInactive = "auth.user_inactive";

        public const string ApiKeyMissing = "auth.apikey.missing";          // 401
        public const string ApiKeyInvalid = "auth.apikey.invalid";          // 401
        public const string ApiKeyRevoked = "auth.apikey.revoked";          // 403
        public const string ApiKeyExpired = "auth.apikey.expired";          // 401
        public const string ApiKeyScopeForbidden = "auth.apikey.scope_forbidden"; // 403
        public const string ApiKeyPathForbidden = "auth.apikey.path_forbidden";   // 403
        public const string ApiKeyEnvironmentMismatch = "auth.apikey.env_mismatch"; // 403
        public const string ApiKeyClientTypeForbidden = "auth.apikey.client_type_forbidden"; // 40
    }

    public static class Common
    {
        public const string NotFound = "common.not_found";
        public const string Conflict = "common.conflict";
    }

    public static class Server
    {
        public const string Error = "server.error";
        public const string Timeout = "server.timeout";
        public const string DependencyFailure = "server.dependency_failure";
        public const string DatabaseError = "server.database_error";
        public const string Cancelled = "server.cancelled";
    }

    public static class Mail
    {
        public const string TemplateMissingData = "mail.tempalte_missingdata";
    }

    public static class User
    {
        public const string EmailAlreadyExist = "user.email_already_exists";
    }

    public static class PreRegistration
    {
        public const string NotFound = "pre_registration.not_found";
        public const string OtpNotFound = "pre_registration.otp_not_found";
        public const string OtpInactive = "pre_registration.otp_inactive";
        public const string OtpExpired = "pre_registration.otp_expired";
        public const string OtpLocked = "pre_registration.otp_locked";
        public const string OtpInvalid = "pre_registration.otp_invalid";
    }


}