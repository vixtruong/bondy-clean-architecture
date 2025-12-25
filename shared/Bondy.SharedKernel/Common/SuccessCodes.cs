namespace Bondy.SharedKernel.Constants;

public static class SuccessCodes
{
    public static class Common
    {
        public const string Ok = "OK";
    }

    public static class Auth
    {
        public const string LoginSuccess = "auth.login.success";
        public const string LogoutSuccess = "auth.logout.success";
        public const string RefreshSuccess = "auth.refresh.success";
    }

    public static class User
    {
        public const string Created = "user.created";
        public const string Updated = "user.updated";
        public const string Deleted = "user.deleted";
        public const string RegisterInit = "user.register_init";
        public const string RegisterVerify = "user.register_verify";
    }
}