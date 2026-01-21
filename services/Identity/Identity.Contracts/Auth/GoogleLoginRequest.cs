using System.ComponentModel.DataAnnotations;

namespace Identity.Contracts.Auth;

public sealed record GoogleLoginRequest([Required] string IdToken);

