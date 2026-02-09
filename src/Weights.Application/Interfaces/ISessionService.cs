using Weights.Application.DTOs.Sessions;

namespace Weights.Application.Interfaces;

public interface ISessionService
{
    Task<SessionResponse> CreateAsync(Guid userId, CreateSessionRequest request, CancellationToken cancellationToken = default);
    Task<SessionHistoryResponse> GetHistoryAsync(Guid userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}
