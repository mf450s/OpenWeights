using Weights.Application.DTOs.Auth;
using Weights.Application.Interfaces;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;

namespace Weights.Application.Services;

public class AuthService(IUnitOfWork unitOfWork, IJwtService jwtService) : IAuthService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.ExistsAsync(request.Email, cancellationToken))
            throw new InvalidOperationException("Email already in use");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse { Id = user.Id, Name = user.Name, Email = user.Email, CreatedAt = user.CreatedAt };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = _jwtService.GenerateToken(user.Id, user.Email);
        var expiresAt = _jwtService.GetTokenExpiration();

        return new LoginResponse { Token = token, ExpiresAt = expiresAt };
    }

    public async Task<RefreshTokenResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        // Validate and extract claims from the existing token (even if expired)
        var principal = _jwtService.GetPrincipalFromToken(request.Token);
        if (principal == null)
            throw new UnauthorizedAccessException("Invalid token");

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var emailClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (userIdClaim == null || emailClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid token claims");

        // Verify user still exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        var newToken = _jwtService.GenerateToken(user.Id, user.Email);
        var expiresAt = _jwtService.GetTokenExpiration();

        return new RefreshTokenResponse { Token = newToken, ExpiresAt = expiresAt };
    }
}
