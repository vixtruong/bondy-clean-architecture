using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Contracts.Auth;

public sealed record GoogleLoginRequest([Required] string IdToken);

