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

    public async Task<TemplateResponse> CreateAsync(Guid userId, CreateTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var template = new WorkoutTemplate
        {
            UserId = userId,
            Name = request.Name,
            Description = request.Description
        };

        template.WorkoutTemplateExercises = request.Exercises.Select(e => new WorkoutTemplateExercise
        {
            ExerciseId = e.ExerciseId,
            OrderIndex = e.OrderIndex,
            TargetSets = e.TargetSets,
            TargetReps = e.TargetReps,
            TargetRPE = e.TargetRPE,
            RestSeconds = e.RestSeconds
        }).ToList();

        await _unitOfWork.WorkoutTemplates.AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with exercises
        var created = await _unitOfWork.WorkoutTemplates.GetWithExercisesAsync(template.Id, cancellationToken);

        return new TemplateResponse
        {
            Id = created!.Id,
            Name = created.Name,
            Description = created.Description,
            Exercises = created.WorkoutTemplateExercises.Select(wte => new TemplateExerciseResponse
            {
                ExerciseId = wte.ExerciseId,
                ExerciseName = wte.Exercise.Name,
                OrderIndex = wte.OrderIndex,
                TargetSets = wte.TargetSets,
                TargetReps = wte.TargetReps,
                TargetRPE = wte.TargetRPE,
                RestSeconds = wte.RestSeconds
            }).ToList()
        };
    }

    public async Task<TemplateResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var template = await _unitOfWork.WorkoutTemplates.GetWithExercisesAsync(id, cancellationToken);

        if (template == null)
            return null;

        return new TemplateResponse
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            Exercises = template.WorkoutTemplateExercises.Select(wte => new TemplateExerciseResponse
            {
                ExerciseId = wte.ExerciseId,
                ExerciseName = wte.Exercise.Name,
                OrderIndex = wte.OrderIndex,
                TargetSets = wte.TargetSets,
                TargetReps = wte.TargetReps,
                TargetRPE = wte.TargetRPE,
                RestSeconds = wte.RestSeconds
            }).ToList()
        };
    }
}
