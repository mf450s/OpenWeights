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
        // Arrange
        var userId = Guid.NewGuid();
        var templateId = 1;
        var request = new CreateSessionRequest
        {
            WorkoutTemplateId = templateId,
            Name = "Chest Day",
            Date = DateTime.UtcNow,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            Note = "Great workout",
            Sets = new List<SetDto>
            {
                new SetDto
                {
                    ExerciseId = 1,
                    SetNumber = 1,
                    Weight = 100,
                    Reps = 10,
                    Rir = 2,
                    DurationSeconds = null,
                    DistanceMeters = null,
                    PerformedAt = DateTime.UtcNow
                }
            }
        };

        var createdSession = new WorkoutSession
        {
            Id = 1,
            UserId = userId,
            WorkoutTemplateId = templateId,
            Name = request.Name,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Note = request.Note
        };

        _mockSessionRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutSession session, CancellationToken ct) =>
            {
                session.Id = 1;
                return Task.FromResult(session);
            });

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sessionService.CreateAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("success", result.Status);
        _mockSessionRepository.Verify(
            x => x.AddAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithMultipleSets_ShouldCreateAllSets()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateSessionRequest
        {
            Name = "Back Day",
            Date = DateTime.UtcNow,
            StartTime = DateTime.UtcNow,
            Sets = new List<SetDto>
            {
                new SetDto
                {
                    ExerciseId = 1,
                    SetNumber = 1,
                    Weight = 100,
                    Reps = 8,
                    Rir = 2,
                    PerformedAt = DateTime.UtcNow
                },
                new SetDto
                {
                    ExerciseId = 1,
                    SetNumber = 2,
                    Weight = 95,
                    Reps = 10,
                    Rir = 2,
                    PerformedAt = DateTime.UtcNow
                },
                new SetDto
                {
                    ExerciseId = 2,
                    SetNumber = 1,
                    Weight = 50,
                    Reps = 12,
                    Rir = 3,
                    PerformedAt = DateTime.UtcNow
                }
            }
        };

        WorkoutSession? capturedSession = null;
        _mockSessionRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutSession session, CancellationToken ct) =>
            {
                session.Id = 1;
                capturedSession = session;
                return Task.FromResult(session);
            });

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sessionService.CreateAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(capturedSession);
        Assert.Equal(3, capturedSession.SetHistories.Count);
        Assert.Equal(1, capturedSession.SetHistories.First().ExerciseId);
        Assert.Equal(2, capturedSession.SetHistories.Last().ExerciseId);
    }

    [Fact]
    public async Task CreateAsync_WithNullEndTime_ShouldCreateSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateSessionRequest
        {
            Name = "Quick Session",
            Date = DateTime.UtcNow,
            StartTime = DateTime.UtcNow,
            EndTime = null,
            Sets = new List<SetDto>
            {
                new SetDto
                {
                    ExerciseId = 1,
                    SetNumber = 1,
                    Weight = 100,
                    Reps = 10,
                    PerformedAt = DateTime.UtcNow
                }
            }
        };

        _mockSessionRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutSession session, CancellationToken ct) =>
            {
                session.Id = 1;
                return Task.FromResult(session);
            });

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sessionService.CreateAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("success", result.Status);
    }

    [Fact]
    public async Task CreateAsync_WithEmptySets_ShouldCreateSessionWithNoSets()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateSessionRequest
        {
            Name = "Empty Session",
            Date = DateTime.UtcNow,
            StartTime = DateTime.UtcNow,
            Sets = new List<SetDto>()
        };

        WorkoutSession? capturedSession = null;
        _mockSessionRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutSession session, CancellationToken ct) =>
            {
                session.Id = 1;
                capturedSession = session;
                return Task.FromResult(session);
            });

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sessionService.CreateAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(capturedSession);
        Assert.Empty(capturedSession.SetHistories);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallSaveChangesWithCancellationToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cts = new CancellationTokenSource();
        var request = new CreateSessionRequest
        {
            Name = "Test",
            Date = DateTime.UtcNow,
            StartTime = DateTime.UtcNow,
            Sets = new List<SetDto>
            {
                new SetDto { ExerciseId = 1, SetNumber = 1, PerformedAt = DateTime.UtcNow }
            }
        };

        _mockSessionRepository
            .Setup(x => x.AddAsync(It.IsAny<WorkoutSession>(), It.IsAny<CancellationToken>()))
            .Returns((WorkoutSession session, CancellationToken ct) =>
            {
                session.Id = 1;
                return Task.FromResult(session);
            });

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sessionService.CreateAsync(userId, request, cts.Token);

        // Assert
        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(cts.Token),
            Times.Once);
    }

    #endregion

    #region GetHistoryAsync Tests

    [Fact]
    public async Task GetHistoryAsync_WithValidParameters_ShouldReturnPaginatedHistory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var page = 1;
        var pageSize = 10;

        var sessions = new List<WorkoutSession>
        {
            new WorkoutSession
            {
                Id = 1,
                UserId = userId,
                Name = "Session 1",
                Date = DateTime.UtcNow.AddDays(-1)
            }
        };

        var sessionWithSets = new WorkoutSession
        {
            Id = 1,
            UserId = userId,
            Name = "Session 1",
            Date = DateTime.UtcNow.AddDays(-1),
            SetHistories = new List<SetHistory>
            {
                new SetHistory { ExerciseId = 1, Weight = 100, Reps = 10 },
                new SetHistory { ExerciseId = 2, Weight = 50, Reps = 12 }
            }
        };

        _mockSessionRepository
            .Setup(x => x.GetByUserIdAsync(userId, page, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);

        _mockSessionRepository
            .Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockSessionRepository
            .Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionWithSets);

        // Act
        var result = await _sessionService.GetHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal(page, result.Page);
        Assert.Equal("Session 1", result.Data.First().Name);
    }

    [Fact]
    public async Task GetHistoryAsync_ShouldCalculateTotalVolumeCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sessions = new List<WorkoutSession>
        {
            new WorkoutSession { Id = 1, UserId = userId, Name = "Test", Date = DateTime.UtcNow }
        };

        var sessionWithSets = new WorkoutSession
        {
            Id = 1,
            SetHistories = new List<SetHistory>
            {
                new SetHistory { Weight = 100, Reps = 10 },  // 100 * 10 = 1000
                new SetHistory { Weight = 80, Reps = 12 }    // 80 * 12 = 960
            }
        };

        _mockSessionRepository
            .Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);

        _mockSessionRepository
            .Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockSessionRepository
            .Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionWithSets);

        // Act
        var result = await _sessionService.GetHistoryAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1960, result.Data.First().TotalVolume);
    }

    [Fact]
    public async Task GetHistoryAsync_WithNullWeightOrReps_ShouldNotIncludeInVolume()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sessions = new List<WorkoutSession>
        {
            new WorkoutSession { Id = 1, UserId = userId, Name = "Test", Date = DateTime.UtcNow }
        };

        var sessionWithSets = new WorkoutSession
        {
            Id = 1,
            SetHistories = new List<SetHistory>
            {
                new SetHistory { Weight = 100, Reps = 10 },   // 100 * 10 = 1000
                new SetHistory { Weight = null, Reps = 10 },  // Skipped
                new SetHistory { Weight = 80, Reps = null },  // Skipped
                new SetHistory { Weight = 50, Reps = 5 }      // 50 * 5 = 250
            }
        };

        _mockSessionRepository
            .Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkoutSession> { sessions[0] });

        _mockSessionRepository
            .Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockSessionRepository
            .Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionWithSets);

        // Act
        var result = await _sessionService.GetHistoryAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1250, result.Data.First().TotalVolume);
    }

    [Fact]
    public async Task GetHistoryAsync_WithNoSets_ShouldReturnZeroVolume()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sessions = new List<WorkoutSession>
        {
            new WorkoutSession { Id = 1, UserId = userId, Name = "Test", Date = DateTime.UtcNow }
        };

        var sessionWithSets = new WorkoutSession
        {
            Id = 1,
            SetHistories = new List<SetHistory>()
        };

        _mockSessionRepository
            .Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);

        _mockSessionRepository
            .Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockSessionRepository
            .Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionWithSets);

        // Act
        var result = await _sessionService.GetHistoryAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(0, result.Data.First().TotalVolume);
    }

    [Fact]
    public async Task GetHistoryAsync_WithNullSessionFromRepository_ShouldReturnZeroVolume()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sessions = new List<WorkoutSession>
        {
            new WorkoutSession { Id = 1, UserId = userId, Name = "Test", Date = DateTime.UtcNow }
        };

        _mockSessionRepository
            .Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);

        _mockSessionRepository
            .Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockSessionRepository
            .Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutSession?)null);

        // Act
        var result = await _sessionService.GetHistoryAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(0, result.Data.First().TotalVolume);
    }

    [Fact]
    public async Task GetHistoryAsync_WithMultipleSessions_ShouldReturnAllWithCorrectVolume()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sessions = new List<WorkoutSession>
        {
            new WorkoutSession { Id = 1, UserId = userId, Name = "Session 1", Date = DateTime.UtcNow.AddDays(-2) },
            new WorkoutSession { Id = 2, UserId = userId, Name = "Session 2", Date = DateTime.UtcNow.AddDays(-1) }
        };

        var session1WithSets = new WorkoutSession
        {
            Id = 1,
            SetHistories = new List<SetHistory>
            {
                new SetHistory { Weight = 100, Reps = 10 }
            }
        };

        var session2WithSets = new WorkoutSession
        {
            Id = 2,
            SetHistories = new List<SetHistory>
            {
                new SetHistory { Weight = 150, Reps = 5 }
            }
        };

        _mockSessionRepository
            .Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);

        _mockSessionRepository
            .Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        _mockSessionRepository
            .Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session1WithSets);

        _mockSessionRepository
            .Setup(x => x.GetWithSetsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session2WithSets);

        // Act
        var result = await _sessionService.GetHistoryAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(1000, result.Data[0].TotalVolume);
        Assert.Equal(750, result.Data[1].TotalVolume);
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetHistoryAsync_WithPagination_ShouldRespectPageParameters()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var page = 2;
        var pageSize = 5;
        var sessions = new List<WorkoutSession>
        {
            new WorkoutSession { Id = 1, UserId = userId, Name = "Session 1", Date = DateTime.UtcNow }
        };

        _mockSessionRepository
            .Setup(x => x.GetByUserIdAsync(userId, page, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);

        _mockSessionRepository
            .Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(15);

        var sessionWithSets = new WorkoutSession
        {
            Id = 1,
            SetHistories = new List<SetHistory>()
        };

        _mockSessionRepository
            .Setup(x => x.GetWithSetsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessionWithSets);

        // Act
        var result = await _sessionService.GetHistoryAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(page, result.Page);
        Assert.Equal(15, result.TotalCount);
        _mockSessionRepository.Verify(
            x => x.GetByUserIdAsync(userId, page, pageSize, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetHistoryAsync_WithEmptyResult_ShouldReturnEmptyData()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockSessionRepository
            .Setup(x => x.GetByUserIdAsync(userId, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkoutSession>());

        _mockSessionRepository
            .Setup(x => x.GetTotalCountByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _sessionService.GetHistoryAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalCount);
    }

    #endregion
}
