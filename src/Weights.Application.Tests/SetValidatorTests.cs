using FluentValidation.TestHelper;
using Moq;
using Weights.Application.DTOs.Sessions;
using Weights.Application.Validators;
using Weights.Domain.Entities;
using Weights.Domain.Enums;
using Weights.Domain.Interfaces;
using Xunit;

namespace Weights.Application.Tests;

public class SetValidatorTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SetValidator _validator;

    public SetValidatorTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validator = new SetValidator(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Validate_UnilateralExercise_WithoutSide_ShouldFail()
    {
        // Arrange
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Dumbbell Curl",
            Laterality = Laterality.Unilateral,
            TrackType = TrackType.WeightReps
        };

        _unitOfWorkMock.Setup(x => x.Exercises.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var setDto = new SetDto
        {
            ExerciseId = 1,
            SetNumber = 1,
            Weight = 10,
            Reps = 10,
            Side = null,
            PerformedAt = DateTime.UtcNow
        };

        // Act
        var result = await _validator.TestValidateAsync(setDto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public async Task Validate_UnilateralExercise_WithSideLeft_ShouldPass()
    {
        // Arrange
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Dumbbell Curl",
            Laterality = Laterality.Unilateral,
            TrackType = TrackType.WeightReps
        };

        _unitOfWorkMock.Setup(x => x.Exercises.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var setDto = new SetDto
        {
            ExerciseId = 1,
            SetNumber = 1,
            Weight = 10,
            Reps = 10,
            Side = "Left",
            PerformedAt = DateTime.UtcNow
        };

        // Act
        var result = await _validator.TestValidateAsync(setDto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public async Task Validate_UnilateralExercise_WithSideRight_ShouldPass()
    {
        // Arrange
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Dumbbell Curl",
            Laterality = Laterality.Unilateral,
            TrackType = TrackType.WeightReps
        };

        _unitOfWorkMock.Setup(x => x.Exercises.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var setDto = new SetDto
        {
            ExerciseId = 1,
            SetNumber = 1,
            Weight = 10,
            Reps = 10,
            Side = "Right",
            PerformedAt = DateTime.UtcNow
        };

        // Act
        var result = await _validator.TestValidateAsync(setDto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public async Task Validate_BilateralExercise_WithSideLeft_ShouldPass()
    {
        // Arrange
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Bench Press",
            Laterality = Laterality.Bilateral,
            TrackType = TrackType.WeightReps
        };

        _unitOfWorkMock.Setup(x => x.Exercises.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var setDto = new SetDto
        {
            ExerciseId = 1,
            SetNumber = 1,
            Weight = 60,
            Reps = 10,
            Side = "Left",
            PerformedAt = DateTime.UtcNow
        };

        // Act
        var result = await _validator.TestValidateAsync(setDto);

        // Assert - Side is ignored for bilateral exercises
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public async Task Validate_BilateralExercise_WithoutSide_ShouldPass()
    {
        // Arrange
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Bench Press",
            Laterality = Laterality.Bilateral,
            TrackType = TrackType.WeightReps
        };

        _unitOfWorkMock.Setup(x => x.Exercises.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var setDto = new SetDto
        {
            ExerciseId = 1,
            SetNumber = 1,
            Weight = 60,
            Reps = 10,
            Side = null,
            PerformedAt = DateTime.UtcNow
        };

        // Act
        var result = await _validator.TestValidateAsync(setDto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public async Task Validate_UnilateralExercise_WithInvalidSide_ShouldFail()
    {
        // Arrange
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Dumbbell Curl",
            Laterality = Laterality.Unilateral,
            TrackType = TrackType.WeightReps
        };

        _unitOfWorkMock.Setup(x => x.Exercises.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var setDto = new SetDto
        {
            ExerciseId = 1,
            SetNumber = 1,
            Weight = 10,
            Reps = 10,
            Side = "InvalidSide",
            PerformedAt = DateTime.UtcNow
        };

        // Act
        var result = await _validator.TestValidateAsync(setDto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x);
    }
}
