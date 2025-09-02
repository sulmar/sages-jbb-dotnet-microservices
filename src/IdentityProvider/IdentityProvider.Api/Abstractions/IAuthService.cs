using Microsoft.AspNetCore.Authorization;

namespace IdentityProvider.Api.Abstractions;

public interface IAuthService
{
    AuthenticationResult Authorize(string username, string password);
}

public record AuthenticationResult(bool IsAuthenticated, UserIdentity? identity = null);

public class UserIdentity
{
    public int Id { get; set; }
    public string UserName { get; set; }= string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public string Email { get; set; }
}


public interface ITokenService
{
    string GenerateAccessToken(UserIdentity identity);
}
