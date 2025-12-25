using Bondy.SharedKernel.Common;

namespace Bondy.ServiceDefaults.Http
{
    public static class HttpStatusMapper
    {
        public static int ToStatusCode(this ErrorType type) => type switch
        {
            ErrorType.Validation => 400,
            ErrorType.NotFound => 404,
            ErrorType.Conflict => 409,
            ErrorType.Unauthorized => 401,
            ErrorType.Forbidden => 403,
            ErrorType.BadRequest => 400,
            _ => 500
        };
    }
}
