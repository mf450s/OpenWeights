using Moq;
using Weights.Application.Services;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;

namespace Weights.Application.Tests;

public class MuscleServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMuscleRepository> _mockMuscleRepository;
    private readonly MuscleService _muscleService;

    public MuscleServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMuscleRepository = new Mock<IMuscleRepository>();
        _mockUnitOfWork.Setup(x => x.Muscles).Returns(_mockMuscleRepository.Object);
        _muscleService = new MuscleService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMuscles()
    {
        var muscles = new List<Muscle>
        {
            new() { Id = 1, Name = "Pectoralis Major", BodyPart = "Chest" },
            new() { Id = 2, Name = "Biceps Brachii", BodyPart = "Arms" }
        };
        _mockMuscleRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(muscles);

        var result = (await _muscleService.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Pectoralis Major", result[0].Name);
        Assert.Equal("Chest", result[0].BodyPart);
        Assert.Equal("Biceps Brachii", result[1].Name);
    }

    [Fact]
    public async Task GetAllAsync_WithNoMuscles_ShouldReturnEmptyList()
    {
        _mockMuscleRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Muscle>());

        var result = await _muscleService.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldMapBodyPartCorrectly()
    {
        var muscles = new List<Muscle> { new() { Id = 1, Name = "Quadriceps", BodyPart = "Legs" } };
        _mockMuscleRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(muscles);

        var result = (await _muscleService.GetAllAsync()).First();

        Assert.Equal("Legs", result.BodyPart);
    }
}
