using Weights.Application.DTOs.Exercises;

namespace Weights.Application.Interfaces;

public interface IExerciseService
{
    Task<IEnumerable<ExerciseResponse>> GetAllAsync(int? muscleId = null, CancellationToken cancellationToken = default);
    Task<ExerciseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
