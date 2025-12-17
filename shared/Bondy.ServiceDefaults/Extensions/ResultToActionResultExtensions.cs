using Bondy.ServiceDefaults.Contracts;
using Bondy.ServiceDefaults.Errors;
using Bondy.SharedKernel.Common;
using Microsoft.AspNetCore.Mvc;

namespace Bondy.ServiceDefaults.Extensions;

public static class ResultToActionResultExtensions
{
    public static ActionResult ToActionResult(this ControllerBase c, Result result)
    {
        var traceId = c.HttpContext.TraceIdentifier;

        if (result.IsSuccess)
        {
            var ok = new ApiResponse(true, result.SuccessCode, null, null, traceId);
            return c.Ok(ok);
        }

        var err = result.Error;
        var status = ErrorMapping.ToStatusCode(err.Type);

        var payload = new ApiResponse(
            Success: false,
            Code: null,
            Data: null,
            Error: new ApiError(err.Code, err.Message, err.Type.ToString(), err.Meta),
            TraceId: traceId
        );

        return c.StatusCode(status, payload);
    }

    public static ActionResult ToActionResult<T>(this ControllerBase c, Result<T> result)
    {
        var traceId = c.HttpContext.TraceIdentifier;

        if (result.IsSuccess)
        {
            var ok = new ApiResponse(true, result.SuccessCode, result.Value, null, traceId);
            return c.Ok(ok);
        }

        var err = result.Error;
        var status = ErrorMapping.ToStatusCode(err.Type);

        var payload = new ApiResponse(
            Success: false,
            Code: null,
            Data: null,
            Error: new ApiError(err.Code, err.Message, err.Type.ToString(), err.Meta),
            TraceId: traceId
        );

        return c.StatusCode(status, payload);
    }
}