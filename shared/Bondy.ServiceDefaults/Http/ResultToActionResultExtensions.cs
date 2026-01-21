using Bondy.SharedKernel.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Bondy.ServiceDefaults.Http;

public static class ResultToActionResultExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result is null) throw new ArgumentNullException(nameof(result));

        var body = ApiResponse<T>.From(result);

        if (result.IsSuccess)
            return controller.Ok(body);

        // Map ErrorType -> HTTP status code
        return controller.StatusCode(result.Error.Type.ToStatusCode(), body);
    }

    public static IActionResult ToActionResult(this ControllerBase controller, Result result)
    {
        if (result is null) throw new ArgumentNullException(nameof(result));

        var body = ApiResponse.From(result);

        if (result.IsSuccess)
            return controller.Ok(body);

        return controller.StatusCode(result.Error.Type.ToStatusCode(), body);
    }
}