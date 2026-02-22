using Weights.Application.DTOs.Sessions;
using Weights.Application.Interfaces;
using Weights.Domain.Entities;
using Weights.Domain.Enums;
using Weights.Domain.Interfaces;

namespace Weights.Application.Services;

public class SessionService(IUnitOfWork unitOfWork) : ISessionService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<SessionResponse> CreateAsync(Guid userId, CreateSessionRequest request, CancellationToken cancellationToken = default)
    {
        var session = new WorkoutSession
        {
            UserId = userId,
            WorkoutTemplateId = request.WorkoutTemplateId,
            Name = request.Name,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Note = request.Note,
            SetHistories = request.Sets.Select(s => new SetHistory
            {
                ExerciseId = s.ExerciseId,
                SetNumber = s.SetNumber,
                Weight = s.Weight,
                Reps = s.Reps,
                RIR = s.Rir,
                DurationSeconds = s.DurationSeconds,
                DistanceMeters = s.DistanceMeters,
                Side = !string.IsNullOrWhiteSpace(s.Side) && Enum.TryParse<Side>(s.Side, true, out var side) ? side : null,
                PerformedAt = s.PerformedAt
            }).ToList()
        };

        await _unitOfWork.WorkoutSessions.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SessionResponse { Id = session.Id, Status = "success" };
    }

    public async Task<SessionDetailResponse?> GetByIdAsync(int id, Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await _unitOfWork.WorkoutSessions.GetWithSetsAsync(id, cancellationToken);

        if (session == null || session.UserId != userId)
            return null;

        return MapToDetail(session);
    }

    public async Task<SessionDetailResponse?> UpdateAsync(int id, Guid userId, UpdateSessionRequest request, CancellationToken cancellationToken = default)
    {
        var session = await _unitOfWork.WorkoutSessions.GetByIdAsync(id, cancellationToken);

        if (session == null || session.UserId != userId)
            return null;

        if (request.Name != null) session.Name = request.Name;
        if (request.EndTime.HasValue) session.EndTime = request.EndTime;
        if (request.Note != null) session.Note = request.Note;

        await _unitOfWork.WorkoutSessions.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _unitOfWork.WorkoutSessions.GetWithSetsAsync(id, cancellationToken);
        return MapToDetail(updated!);
    }

    public async Task<SessionHistoryResponse> GetHistoryAsync(Guid userId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var sessions = await _unitOfWork.WorkoutSessions.GetByUserIdAsync(userId, page, pageSize, cancellationToken);
        var totalCount = await _unitOfWork.WorkoutSessions.GetTotalCountByUserIdAsync(userId, cancellationToken);

        var data = new List<SessionHistoryItem>();

        foreach (var session in sessions)
        {
            var sessionWithSets = await _unitOfWork.WorkoutSessions.GetWithSetsAsync(session.Id, cancellationToken);
            var totalVolume = sessionWithSets?.SetHistories
                .Where(s => s.Weight.HasValue && s.Reps.HasValue)
                .Sum(s => s.Weight!.Value * s.Reps!.Value) ?? 0;

            data.Add(new SessionHistoryItem
            {
                Id = session.Id,
                Name = session.Name,
                Date = session.Date,
                TotalVolume = totalVolume
            });
        }

        return new SessionHistoryResponse { Data = data, TotalCount = totalCount, Page = page };
    }

    private static SessionDetailResponse MapToDetail(WorkoutSession session) => new()
    {
        Id = session.Id,
        Name = session.Name,
        WorkoutTemplateId = session.WorkoutTemplateId,
        Date = session.Date,
        StartTime = session.StartTime,
        EndTime = session.EndTime,
        Note = session.Note,
        Sets = session.SetHistories.Select(sh => new SessionSetResponse
        {
            Id = sh.Id,
            ExerciseId = sh.ExerciseId,
            ExerciseName = sh.Exercise?.Name ?? string.Empty,
            SetNumber = sh.SetNumber,
            Weight = sh.Weight,
            Reps = sh.Reps,
            RIR = sh.RIR,
            DurationSeconds = sh.DurationSeconds,
            DistanceMeters = sh.DistanceMeters,
            Side = sh.Side?.ToString(),
            WorkoutTemplateExerciseId = sh.WorkoutTemplateExerciseId,
            PerformedAt = sh.PerformedAt
        }).OrderBy(s => s.SetNumber).ToList()
    };
}
