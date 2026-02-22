using System.ComponentModel.DataAnnotations;

namespace Weights.Application.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string Token { get; set; } = string.Empty;
}
