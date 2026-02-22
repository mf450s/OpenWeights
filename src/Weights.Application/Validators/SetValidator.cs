using FluentValidation;
using Weights.Application.DTOs.Sessions;
using Weights.Domain.Enums;
using Weights.Domain.Interfaces;

namespace Weights.Application.Validators;

public class SetValidator : AbstractValidator<SetDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.ExerciseId)
            .NotEmpty()
            .WithMessage("ExerciseId is required.");

        RuleFor(x => x.SetNumber)
            .GreaterThan(0)
            .WithMessage("SetNumber must be greater than 0.");

        RuleFor(x => x.Rir)
            .InclusiveBetween(0, 10)
            .When(x => x.Rir.HasValue)
            .WithMessage("RIR must be between 0 and 10.");

        RuleFor(x => x)
            .MustAsync(ValidateSideForLaterality)
            .WithMessage("Side must be specified (Left or Right) for unilateral exercises.");
    }

    private async Task<bool> ValidateSideForLaterality(SetDto dto, CancellationToken cancellationToken)
    {
        var exercise = await _unitOfWork.Exercises.GetByIdAsync(dto.ExerciseId, cancellationToken);

        if (exercise == null)
            return true; // Let ExerciseId validation handle this

        // If exercise is unilateral, Side must be specified
        if (exercise.Laterality == Laterality.Unilateral)
        {
            if (string.IsNullOrWhiteSpace(dto.Side))
                return false;

            // Validate that Side is either "Left" or "Right"
            if (!Enum.TryParse<Side>(dto.Side, true, out _))
                return false;
        }

        return true;
    }
}
