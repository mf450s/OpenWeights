using Weights.Application.DTOs.Common;
using Weights.Application.DTOs.Exercises;
using Weights.Application.Interfaces;
using Weights.Domain.Interfaces;

namespace Weights.Application.Services;

public class ExerciseService(IUnitOfWork unitOfWork) : IExerciseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<PagedResponse<ExerciseResponse>> GetAllAsync(int? muscleId = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var allExercises = muscleId.HasValue
            ? await _unitOfWork.Exercises.GetByMuscleIdAsync(muscleId.Value, cancellationToken)
            : await _unitOfWork.Exercises.GetAllWithMusclesAsync(cancellationToken);

        var exerciseList = allExercises.ToList();
        var totalCount = exerciseList.Count;
        var paged = exerciseList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new ExerciseResponse
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
            }).ToList();

        return new PagedResponse<ExerciseResponse>
        {
            Data = paged,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
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

    public async Task<ExerciseHistoryResponse?> GetHistoryAsync(int exerciseId, Guid userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(exerciseId, cancellationToken);
        if (exercise == null)
            return null;

        var sessions = await _unitOfWork.WorkoutSessions.GetByUserIdAsync(userId, 1, int.MaxValue, cancellationToken);
        var allSessions = sessions.ToList();

        var groups = new List<ExerciseHistorySetGroup>();

        foreach (var session in allSessions.OrderByDescending(s => s.Date))
        {
            var withSets = await _unitOfWork.WorkoutSessions.GetWithSetsAsync(session.Id, cancellationToken);
            var relevantSets = withSets?.SetHistories
                .Where(sh => sh.ExerciseId == exerciseId)
                .OrderBy(sh => sh.SetNumber)
                .ToList();

            if (relevantSets == null || !relevantSets.Any())
                continue;

            groups.Add(new ExerciseHistorySetGroup
            {
                SessionId = session.Id,
                Date = session.Date,
                Sets = relevantSets.Select(sh => new ExerciseHistorySet
                {
                    SetNumber = sh.SetNumber,
                    Weight = sh.Weight,
                    Reps = sh.Reps,
                    RIR = sh.RIR,
                    DurationSeconds = sh.DurationSeconds,
                    DistanceMeters = sh.DistanceMeters
                }).ToList()
            });
        }

        var pagedGroups = groups.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new ExerciseHistoryResponse
        {
            ExerciseId = exerciseId,
            ExerciseName = exercise.Name,
            Sessions = pagedGroups
        };
    }
}
