using Weights.Application.DTOs.Common;
using Weights.Application.DTOs.Exercises;

namespace Weights.Application.Interfaces;

public interface IExerciseService
{
    Task<PagedResponse<ExerciseResponse>> GetAllAsync(int? muscleId = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<ExerciseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ExerciseHistoryResponse?> GetHistoryAsync(int exerciseId, Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}
