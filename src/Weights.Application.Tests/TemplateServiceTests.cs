using Moq;
using Weights.Application.DTOs.Templates;
using Weights.Application.Services;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;

namespace Weights.Application.Tests;

public class TemplateServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IWorkoutTemplateRepository> _mockTemplateRepository;
    private readonly TemplateService _templateService;

    public TemplateServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTemplateRepository = new Mock<IWorkoutTemplateRepository>();
        _mockUnitOfWork.Setup(x => x.WorkoutTemplates).Returns(_mockTemplateRepository.Object);
        _templateService = new TemplateService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldMapTargetRepsMinMaxAndIsAmrap()
    {
        var userId = Guid.NewGuid();
        var request = new CreateTemplateRequest
        {
            Name = "Push Day",
            Exercises = new List<TemplateExerciseDto>
            {
                new() { ExerciseId = 1, OrderIndex = 1, TargetSets = 3, TargetRepsMin = 8, TargetRepsMax = 12, IsAMRAP = false }
            }
        };

        WorkoutTemplate? captured = null;
        _mockTemplateRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutTemplate>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutTemplate t, CancellationToken _) => { t.Id = 1; captured = t; return Task.FromResult(t); });
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockTemplateRepository
            .Setup(x => x.GetWithExercisesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => captured);

        await _templateService.CreateAsync(userId, request);

        Assert.NotNull(captured);
        var wte = captured.WorkoutTemplateExercises.First();
        Assert.Equal(8, wte.TargetRepsMin);
        Assert.Equal(12, wte.TargetRepsMax);
        Assert.False(wte.IsAMRAP);
    }

    [Fact]
    public async Task CreateAsync_WithAmrap_ShouldSetIsAmrapTrue()
    {
        var userId = Guid.NewGuid();
        var request = new CreateTemplateRequest
        {
            Name = "AMRAP Day",
            Exercises = new List<TemplateExerciseDto>
            {
                new() { ExerciseId = 1, OrderIndex = 1, TargetSets = 1, IsAMRAP = true }
            }
        };

        WorkoutTemplate? captured = null;
        _mockTemplateRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutTemplate>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutTemplate t, CancellationToken _) => { t.Id = 1; captured = t; return Task.FromResult(t); });
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockTemplateRepository.Setup(x => x.GetWithExercisesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(() => captured);

        await _templateService.CreateAsync(userId, request);

        Assert.True(captured!.WorkoutTemplateExercises.First().IsAMRAP);
    }

    [Fact]
    public async Task CreateAsync_NullableReps_ShouldAllowNullTargetRepsMinMax()
    {
        var userId = Guid.NewGuid();
        var request = new CreateTemplateRequest
        {
            Name = "Duration Day",
            Exercises = new List<TemplateExerciseDto>
            {
                new() { ExerciseId = 1, OrderIndex = 1, TargetSets = 3, TargetRepsMin = null, TargetRepsMax = null }
            }
        };

        WorkoutTemplate? captured = null;
        _mockTemplateRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutTemplate>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutTemplate t, CancellationToken _) => { t.Id = 1; captured = t; return Task.FromResult(t); });
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockTemplateRepository.Setup(x => x.GetWithExercisesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(() => captured);

        await _templateService.CreateAsync(userId, request);

        var wte = captured!.WorkoutTemplateExercises.First();
        Assert.Null(wte.TargetRepsMin);
        Assert.Null(wte.TargetRepsMax);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentTemplate_ShouldReturnNull()
    {
        _mockTemplateRepository.Setup(x => x.GetWithExercisesAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((WorkoutTemplate?)null);

        var result = await _templateService.UpdateAsync(Guid.NewGuid(), 99, new UpdateTemplateRequest { Name = "X", Exercises = new() });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithWrongUser_ShouldReturnNull()
    {
        var ownerId = Guid.NewGuid();
        var template = new WorkoutTemplate { Id = 1, UserId = ownerId, Name = "T", WorkoutTemplateExercises = new List<WorkoutTemplateExercise>() };
        _mockTemplateRepository.Setup(x => x.GetWithExercisesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(template);

        var result = await _templateService.UpdateAsync(Guid.NewGuid(), 1, new UpdateTemplateRequest { Name = "X", Exercises = new() });

        Assert.Null(result);
    }

    [Fact]
    public async Task ArchiveAsync_WithValidOwner_ShouldReturnTrue()
    {
        var userId = Guid.NewGuid();
        var template = new WorkoutTemplate { Id = 1, UserId = userId, Name = "T", IsArchived = false };
        _mockTemplateRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(template);
        _mockTemplateRepository.Setup(x => x.UpdateAsync(It.IsAny<WorkoutTemplate>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _templateService.ArchiveAsync(userId, 1);

        Assert.True(result);
    }

    [Fact]
    public async Task ArchiveAsync_WithWrongUser_ShouldReturnFalse()
    {
        var ownerId = Guid.NewGuid();
        var template = new WorkoutTemplate { Id = 1, UserId = ownerId, Name = "T" };
        _mockTemplateRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(template);

        var result = await _templateService.ArchiveAsync(Guid.NewGuid(), 1);

        Assert.False(result);
    }
}
