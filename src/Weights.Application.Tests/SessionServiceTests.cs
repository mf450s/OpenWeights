using Moq;
using Weights.Application.DTOs.Sessions;
using Weights.Application.Services;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;

namespace Weights.Application.Tests;

public class SessionServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IWorkoutSessionRepository> _mockSessionRepository;
    private readonly SessionService _sessionService;

    public SessionServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSessionRepository = new Mock<IWorkoutSessionRepository>();
        _mockUnitOfWork.Setup(x => x.WorkoutSessions).Returns(_mockSessionRepository.Object);
        _sessionService = new SessionService(_mockUnitOfWork.Object);
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldCreateSessionSuccessfully()
    {
        var userId = Guid.NewGuid();
        var request = new CreateSessionRequest
        {
            WorkoutTemplateId = 1,
            Name = "Chest Day",
            Date = DateTime.UtcNow,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            Note = "Great workout",
            Sets = new List<SetDto>
            {
                new() { ExerciseId = 1, SetNumber = 1, Weight = 100, Reps = 10, Rir = 2, PerformedAt = DateTime.UtcNow }
            }
        };

        _mockSessionRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutSession s, CancellationToken _) => { s.Id = 1; return Task.FromResult(s); });
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sessionService.CreateAsync(userId, request);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("success", result.Status);
        _mockSessionRepository.Verify(x => x.AddAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithMultipleSets_ShouldCreateAllSets()
    {
        var userId = Guid.NewGuid();
        var request = new CreateSessionRequest
        {
            Name = "Back Day", Date = DateTime.UtcNow, StartTime = DateTime.UtcNow,
            Sets = new List<SetDto>
            {
                new() { ExerciseId = 1, SetNumber = 1, Weight = 100, Reps = 8, Rir = 2, PerformedAt = DateTime.UtcNow },
                new() { ExerciseId = 1, SetNumber = 2, Weight = 95, Reps = 10, Rir = 2, PerformedAt = DateTime.UtcNow },
                new() { ExerciseId = 2, SetNumber = 1, Weight = 50, Reps = 12, Rir = 3, PerformedAt = DateTime.UtcNow }
            }
        };

        WorkoutSession? captured = null;
        _mockSessionRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutSession s, CancellationToken _) => { s.Id = 1; captured = s; return Task.FromResult(s); });
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _sessionService.CreateAsync(userId, request);

        Assert.NotNull(captured);
        Assert.Equal(3, captured.SetHistories.Count);
    }

    [Fact]
    public async Task CreateAsync_WithEmptySets_ShouldCreateSessionWithNoSets()
    {
        var userId = Guid.NewGuid();
        var request = new CreateSessionRequest { Name = "Empty", Date = DateTime.UtcNow, StartTime = DateTime.UtcNow, Sets = new() };

        WorkoutSession? captured = null;
        _mockSessionRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutSession s, CancellationToken _) => { s.Id = 1; captured = s; return Task.FromResult(s); });
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sessionService.CreateAsync(userId, request);

        Assert.NotNull(captured);
        Assert.Empty(captured.SetHistories);
        Assert.Equal("success", result.Status);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WithValidIdAndOwner_ShouldReturnDetail()
    {
        var userId = Guid.NewGuid();
        var session = new WorkoutSession
        {
            Id = 1, UserId = userId, Name = "Leg Day",
            Date = DateTime.UtcNow, StartTime = DateTime.UtcNow,
            SetHistories = new List<SetHistory>
            {
                new() { Id = 1, ExerciseId = 1, SetNumber = 1, Weight = 80, Reps = 10, PerformedAt = DateTime.UtcNow }
            }
        };

        _mockSessionRepository.Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(session);

        var result = await _sessionService.GetByIdAsync(1, userId);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Leg Day", result.Name);
        Assert.Single(result.Sets);
    }

    [Fact]
    public async Task GetByIdAsync_WithWrongUser_ShouldReturnNull()
    {
        var ownerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();
        var session = new WorkoutSession { Id = 1, UserId = ownerId, Date = DateTime.UtcNow, StartTime = DateTime.UtcNow, SetHistories = new() };

        _mockSessionRepository.Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(session);

        var result = await _sessionService.GetByIdAsync(1, otherId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        _mockSessionRepository.Setup(x => x.GetWithSetsAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((WorkoutSession?)null);

        var result = await _sessionService.GetByIdAsync(99, Guid.NewGuid());

        Assert.Null(result);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidRequest_ShouldReturnUpdatedSession()
    {
        var userId = Guid.NewGuid();
        var session = new WorkoutSession { Id = 1, UserId = userId, Name = "Old Name", Date = DateTime.UtcNow, StartTime = DateTime.UtcNow, SetHistories = new() };
        var updatedSession = new WorkoutSession { Id = 1, UserId = userId, Name = "New Name", Date = session.Date, StartTime = session.StartTime, Note = "Updated note", SetHistories = new() };
        var request = new UpdateSessionRequest { Name = "New Name", Note = "Updated note" };

        _mockSessionRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(session);
        _mockSessionRepository.Setup(x => x.UpdateAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockSessionRepository.Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(updatedSession);

        var result = await _sessionService.UpdateAsync(1, userId, request);

        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("Updated note", result.Note);
    }

    [Fact]
    public async Task UpdateAsync_WithWrongUser_ShouldReturnNull()
    {
        var ownerId = Guid.NewGuid();
        var session = new WorkoutSession { Id = 1, UserId = ownerId, Date = DateTime.UtcNow, StartTime = DateTime.UtcNow };
        _mockSessionRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(session);

        var result = await _sessionService.UpdateAsync(1, Guid.NewGuid(), new UpdateSessionRequest());

        Assert.Null(result);
    }

    #endregion

    #region GetHistoryAsync Tests

    [Fact]
    public async Task GetHistoryAsync_ShouldCalculateTotalVolumeCorrectly()
    {
        var userId = Guid.NewGuid();
        var sessions = new List<WorkoutSession> { new() { Id = 1, UserId = userId, Name = "Test", Date = DateTime.UtcNow } };
        var sessionWithSets = new WorkoutSession
        {
            Id = 1,
            SetHistories = new List<SetHistory>
            {
                new() { Weight = 100, Reps = 10 },
                new() { Weight = 80, Reps = 12 }
            }
        };

        _mockSessionRepository.Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(sessions);
        _mockSessionRepository.Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockSessionRepository.Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(sessionWithSets);

        var result = await _sessionService.GetHistoryAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(1960, result.Data.First().TotalVolume);
    }

    [Fact]
    public async Task GetHistoryAsync_WithNullWeightOrReps_ShouldNotIncludeInVolume()
    {
        var userId = Guid.NewGuid();
        var sessions = new List<WorkoutSession> { new() { Id = 1, UserId = userId, Name = "Test", Date = DateTime.UtcNow } };
        var sessionWithSets = new WorkoutSession
        {
            Id = 1,
            SetHistories = new List<SetHistory>
            {
                new() { Weight = 100, Reps = 10 },
                new() { Weight = null, Reps = 10 },
                new() { Weight = 80, Reps = null },
                new() { Weight = 50, Reps = 5 }
            }
        };

        _mockSessionRepository.Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(sessions);
        _mockSessionRepository.Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockSessionRepository.Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(sessionWithSets);

        var result = await _sessionService.GetHistoryAsync(userId);

        Assert.Equal(1250, result.Data.First().TotalVolume);
    }

    [Fact]
    public async Task GetHistoryAsync_WithEmptyResult_ShouldReturnEmptyData()
    {
        var userId = Guid.NewGuid();
        _mockSessionRepository.Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(new List<WorkoutSession>());
        _mockSessionRepository.Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var result = await _sessionService.GetHistoryAsync(userId);

        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetHistoryAsync_WithPagination_ShouldPassParametersToRepository()
    {
        var userId = Guid.NewGuid();
        var page = 3;
        var pageSize = 5;
        _mockSessionRepository.Setup(x => x.GetByUserIdAsync(userId, page, pageSize, It.IsAny<CancellationToken>())).ReturnsAsync(new List<WorkoutSession>());
        _mockSessionRepository.Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(20);

        var result = await _sessionService.GetHistoryAsync(userId, page, pageSize);

        Assert.Equal(page, result.Page);
        Assert.Equal(20, result.TotalCount);
        _mockSessionRepository.Verify(x => x.GetByUserIdAsync(userId, page, pageSize, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
