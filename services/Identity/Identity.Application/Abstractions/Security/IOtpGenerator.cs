namespace Identity.Application.Abstractions.Security;

public interface IOtpGenerator
{
    string Generate(int length = 6);
}
