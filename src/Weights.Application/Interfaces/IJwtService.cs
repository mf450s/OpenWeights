using System.Security.Claims;

namespace Weights.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email);
    DateTime GetTokenExpiration();
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}
