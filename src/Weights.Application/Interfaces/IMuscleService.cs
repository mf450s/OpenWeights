using Weights.Application.DTOs.Muscles;

namespace Weights.Application.Interfaces;

public interface IMuscleService
{
    Task<IEnumerable<MuscleResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MuscleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<MuscleResponse> CreateAsync(MuscleCreateRequest request, CancellationToken cancellationToken = default);    
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<MuscleResponse?> UpdateAsync(int id, MuscleUpdateRequest request, CancellationToken cancellationToken = default);
}
