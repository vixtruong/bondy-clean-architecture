using Bondy.SharedKernel.Application.Abstractions.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Bondy.SharedKernel.Infrastructure.Security;

public sealed class HttpContextCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _context;

    public HttpContextCurrentUser(IHttpContextAccessor context)
    {
        _context = context;
    }

    public bool IsAuthenticated =>
        _context.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public long UserId =>
        long.Parse(
            _context.HttpContext!
                .User
                .FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public string? Email =>
        _context.HttpContext?.User
            .FindFirst(ClaimTypes.Email)
            ?.Value;
}

