using Weights.Application.DTOs.Muscles;

namespace Weights.Application.Interfaces;

public interface IMuscleService
{
    Task<IEnumerable<MuscleResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
