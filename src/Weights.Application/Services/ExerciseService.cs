using System.Security.Cryptography.X509Certificates;
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
                Muscles = [.. e.ExerciseMuscles.Select(em => new ExerciseMuscleDto
                {
                    Id = em.Muscle.Id,
                    Name = em.Muscle.Name,
                    TargetType = em.TargetType.ToString()
                })]
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
            Muscles = [.. exercise.ExerciseMuscles.Select(em => new ExerciseMuscleDto
            {
                Id = em.Muscle.Id,
                Name = em.Muscle.Name,
                TargetType = em.TargetType.ToString()
            })]
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
                Sets = [.. relevantSets.Select(sh => new ExerciseHistorySet
                {
                    SetNumber = sh.SetNumber,
                    Weight = sh.Weight,
                    Reps = sh.Reps,
                    RIR = sh.RIR,
                    DurationSeconds = sh.DurationSeconds,
                    DistanceMeters = sh.DistanceMeters
                })]
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

    public async Task<bool> DeleteAsync(int id, Guid userId, CancellationToken cancellationToken = default)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(id, cancellationToken);
        if (exercise == null)
            return false;

        // Check if the exercise is used in any workout session sets
        var sessions = await _unitOfWork.WorkoutSessions.GetByUserIdAsync(userId, 1, int.MaxValue, cancellationToken);
        foreach (var session in sessions)
        {
            var withSets = await _unitOfWork.WorkoutSessions.GetWithSetsAsync(session.Id, cancellationToken);
            if (withSets?.SetHistories.Any(sh => sh.ExerciseId == id) == true)
                throw new InvalidOperationException("Cannot delete exercise that has been used in workout sessions.");
        }

        await _unitOfWork.Exercises.DeleteAsync(exercise);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task CreateAsync(ExerciseCreateRequest request, CancellationToken cancellationToken = default)
    {
        var exercise = new Domain.Entities.Exercise
        {
            Name = request.Name,
            TrackType = request.TrackType,
            Laterality = request.Laterality
        };

        foreach (var muscle in request.ExerciseMuscles)
        {
            exercise.ExerciseMuscles.Add(new Domain.Entities.ExerciseMuscle
            {
                MuscleId = muscle.MuscleId,
                TargetType = muscle.TargetType
            });
        }

        await _unitOfWork.Exercises.AddAsync(exercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ExerciseResponse> CreateAsync(ExerciseCreateRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var exercise = new Domain.Entities.Exercise
        {
            Name = request.Name,
            TrackType = request.TrackType,
            Laterality = request.Laterality
        };

        foreach (var muscle in request.ExerciseMuscles)
        {
            exercise.ExerciseMuscles.Add(new Domain.Entities.ExerciseMuscle
            {
                MuscleId = muscle.MuscleId,
                TargetType = muscle.TargetType
            });
        }

        await _unitOfWork.Exercises.AddAsync(exercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ExerciseResponse
        {
            Id = exercise.Id,
            Name = exercise.Name,
            TrackType = exercise.TrackType.ToString(),
            Laterality = exercise.Laterality.ToString(),
            Muscles = [.. exercise.ExerciseMuscles.Select(em => new ExerciseMuscleDto
            {
                Id = em.Muscle.Id,
                Name = em.Muscle.Name,
                TargetType = em.TargetType.ToString()
            })]
        };
    }
}
