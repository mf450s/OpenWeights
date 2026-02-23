using Weights.Application.DTOs.Templates;
using Weights.Application.Interfaces;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;

namespace Weights.Application.Services;

public class TemplateService(IUnitOfWork unitOfWork) : ITemplateService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<WorkoutTemplateListResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var templates = await _unitOfWork.WorkoutTemplates.GetByUserIdAsync(userId, cancellationToken);

        return templates.Select(t => new WorkoutTemplateListResponse
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            ExerciseCount = t.WorkoutTemplateExercises.Count
        });
    }

    public async Task<TemplateResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.WorkoutTemplates.GetWithExercisesAsync(id, cancellationToken);

        if (template == null)
            return null;

        return MapToResponse(template);
    }

    public async Task<TemplateResponse> CreateAsync(Guid userId, CreateTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var template = new WorkoutTemplate
        {
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            WorkoutTemplateExercises = [.. request.Exercises.Select(e => new WorkoutTemplateExercise
            {
                ExerciseId = e.ExerciseId,
                OrderIndex = e.OrderIndex,
                TargetSets = e.TargetSets,
                TargetRepsMin = e.TargetRepsMin,
                TargetRepsMax = e.TargetRepsMax,
                IsAMRAP = e.IsAMRAP,
                TargetRPE = e.TargetRPE,
                RestSeconds = e.RestSeconds
            })]
        };

        await _unitOfWork.WorkoutTemplates.AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.WorkoutTemplates.GetWithExercisesAsync(template.Id, cancellationToken);
        return MapToResponse(created!);
    }

    public async Task<TemplateResponse?> UpdateAsync(Guid userId, int id, UpdateTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.WorkoutTemplates.GetWithExercisesAsync(id, cancellationToken);

        if (template == null || template.UserId != userId || template.IsArchived)
            return null;

        template.Name = request.Name;
        template.Description = request.Description;
        template.UpdatedAt = DateTime.UtcNow;

        // Replace exercises
        template.WorkoutTemplateExercises = [.. request.Exercises.Select(e => new WorkoutTemplateExercise
        {
            ExerciseId = e.ExerciseId,
            OrderIndex = e.OrderIndex,
            TargetSets = e.TargetSets,
            TargetRepsMin = e.TargetRepsMin,
            TargetRepsMax = e.TargetRepsMax,
            IsAMRAP = e.IsAMRAP,
            TargetRPE = e.TargetRPE,
            RestSeconds = e.RestSeconds
        })];

        await _unitOfWork.WorkoutTemplates.UpdateAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _unitOfWork.WorkoutTemplates.GetWithExercisesAsync(id, cancellationToken);
        return MapToResponse(updated!);
    }

    public async Task<bool> ArchiveAsync(Guid userId, int id, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.WorkoutTemplates.GetByIdAsync(id, cancellationToken);

        if (template == null || template.UserId != userId)
            return false;

        template.IsArchived = true;
        template.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.WorkoutTemplates.UpdateAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static TemplateResponse MapToResponse(WorkoutTemplate template) => new()
    {
        Id = template.Id,
        Name = template.Name,
        Description = template.Description,
        IsArchived = template.IsArchived,
        UpdatedAt = template.UpdatedAt,
        Exercises = [.. template.WorkoutTemplateExercises.Select(wte => new TemplateExerciseResponse
        {
            Id = wte.Id,
            ExerciseId = wte.ExerciseId,
            ExerciseName = wte.Exercise?.Name ?? string.Empty,
            OrderIndex = wte.OrderIndex,
            TargetSets = wte.TargetSets,
            TargetRepsMin = wte.TargetRepsMin,
            TargetRepsMax = wte.TargetRepsMax,
            IsAMRAP = wte.IsAMRAP,
            TargetRPE = wte.TargetRPE,
            RestSeconds = wte.RestSeconds
        }).OrderBy(e => e.OrderIndex)]
    };
}
