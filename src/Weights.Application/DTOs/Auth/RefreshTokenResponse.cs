namespace Weights.Application.DTOs.Auth;

public class RefreshTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
