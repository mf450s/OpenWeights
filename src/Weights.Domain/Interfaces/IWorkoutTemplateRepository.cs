using Weights.Domain.Entities;

namespace Weights.Domain.Interfaces;

public interface IWorkoutTemplateRepository : IRepository<WorkoutTemplate, int>
{
    Task<IEnumerable<WorkoutTemplate>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<WorkoutTemplate?> GetWithExercisesAsync(int id, CancellationToken cancellationToken = default);
}
