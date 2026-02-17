using Weights.Application.DTOs.Exercises;
using Weights.Application.Interfaces;
using Weights.Domain.Interfaces;

namespace Weights.Application.Services;

public class ExerciseService(IUnitOfWork unitOfWork) : IExerciseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ExerciseResponse>> GetAllAsync(int? muscleId = null, CancellationToken cancellationToken = default)
    {
        var exercises = muscleId.HasValue
            ? await _unitOfWork.Exercises.GetByMuscleIdAsync(muscleId.Value, cancellationToken)
            : await _unitOfWork.Exercises.GetAllWithMusclesAsync(cancellationToken);

        return exercises.Select(e => new ExerciseResponse
        {
            Id = e.Id,
            Name = e.Name,
            TrackType = e.TrackType.ToString(),
            Laterality = e.Laterality.ToString(),
            Muscles = e.ExerciseMuscles.Select(em => new ExerciseMuscleDto
            {
                Id = em.Muscle.Id,
                Name = em.Muscle.Name,
                TargetType = em.TargetType.ToString()
            }).ToList()
        });
    }

    public async Task<ExerciseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var exercise = await _unitOfWork.Exercises.GetWithMusclesAsync(id, cancellationToken);

        if (exercise == null)
            return null;

        return new ExerciseResponse
        {
            Id = exercise.Id,
            Name = exercise.Name,
            TrackType = exercise.TrackType.ToString(),
            Laterality = exercise.Laterality.ToString(),
            Muscles = exercise.ExerciseMuscles.Select(em => new ExerciseMuscleDto
            {
                Id = em.Muscle.Id,
                Name = em.Muscle.Name,
                TargetType = em.TargetType.ToString()
            }).ToList()
        };
    }
}
