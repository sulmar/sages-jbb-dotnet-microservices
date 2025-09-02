using IdentityProvider.Api.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace IdentityProvider.Api.Infrastructures;

public class AuthService : IAuthService
{
    public AuthenticationResult Authorize(string username, string password)
    {
        if (username == "john" && password == "123")
        {
            var identity = new UserIdentity { Id = 1, UserName = "john", Email ="john@domain.com", HashedPassword = "123" };

            return new AuthenticationResult(IsAuthenticated: true, identity);
        }

        return new AuthenticationResult(IsAuthenticated: false);
    }
}


public class FakeTokenService : ITokenService
{
    public string GenerateAccessToken(UserIdentity identity)
    {
        return "a";
    }
}


// dotnet add package Microsoft.IdentityModel.JsonWebTokens

public class JwtTokenService : ITokenService
{
    public string GenerateAccessToken(UserIdentity identity)
    {
        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            [JwtRegisteredClaimNames.Name] = identity.UserName,
            [ClaimTypes.Email] = identity.Email,
            [ClaimTypes.Role] = new[] { "developer", "admin", "tester" },
            [JwtRegisteredClaimNames.Birthdate] = DateTime.Today.AddYears(-21)
        };

        string secretKey = "a-string-secret-at-least-256-bits-long";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = "https://www.sages.pl",
            Audience = "https://jbb.pl",
            Expires = DateTime.UtcNow.AddMinutes(5),
            Claims = claims,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var jwt_token = new JsonWebTokenHandler().CreateToken(descriptor);
               

        return jwt_token;

    }
}
