using Weights.Application.DTOs.Sessions;
using Weights.Application.Interfaces;
using Weights.Domain.Entities;
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
                PerformedAt = s.PerformedAt
            }).ToList()
        };

        await _unitOfWork.WorkoutSessions.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SessionResponse
        {
            Id = session.Id,
            Status = "success"
        };
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

        return new SessionHistoryResponse
        {
            Data = data,
            TotalCount = totalCount,
            Page = page
        };
    }
}
