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
}