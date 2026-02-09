using Weights.Domain.Entities;

namespace Weights.Domain.Interfaces;

public interface IExerciseRepository : IRepository<Exercise, int>
{
    Task<IEnumerable<Exercise>> GetByMuscleIdAsync(int muscleId, CancellationToken cancellationToken = default);
    Task<Exercise?> GetWithMusclesAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Exercise>> GetAllWithMusclesAsync(CancellationToken cancellationToken = default);
}
