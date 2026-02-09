namespace Weights.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email);
    DateTime GetTokenExpiration();
}
