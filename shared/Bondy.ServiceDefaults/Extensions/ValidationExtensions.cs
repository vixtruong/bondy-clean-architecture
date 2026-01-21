using Bondy.ServiceDefaults.Http;
using Bondy.SharedKernel.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Bondy.ServiceDefaults.Extensions;

public static class ValidationExtensions
{
    public static IMvcBuilder AddServiceValidation(this IMvcBuilder mvc)
    {
        mvc.ConfigureApiBehaviorOptions(opt =>
        {
            opt.InvalidModelStateResponseFactory = ctx =>
            {
                // Lấy toàn bộ message lỗi (distinct để khỏi trùng)
                var messages = ctx.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .Distinct()
                    .ToArray();

                var combinedMessage = messages.Length > 0
                    ? messages[0]
                    : "Validation failed";

                var err = Error.Validation(
                    ErrorCodes.Validation.Argument,
                    combinedMessage
                );

                var payload = new ApiResponse(
                    Success: false,
                    Code: err.Code,
                    Data: null,
                    Error: err,
                    Message: err.Message
                );

                return new BadRequestObjectResult(payload);
            };
        });

        return mvc;
    }
}