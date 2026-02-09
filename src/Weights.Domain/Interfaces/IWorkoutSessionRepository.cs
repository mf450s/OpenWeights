using Weights.Domain.Entities;

namespace Weights.Domain.Interfaces;

public interface IWorkoutSessionRepository : IRepository<WorkoutSession, int>
{
    Task<IEnumerable<WorkoutSession>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<WorkoutSession?> GetWithSetsAsync(int id, CancellationToken cancellationToken = default);
}
