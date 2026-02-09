using Microsoft.EntityFrameworkCore;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;
using Weights.Infrastructure.Data;

namespace Weights.Infrastructure.Repositories;

public class WorkoutSessionRepository : Repository<WorkoutSession, int>, IWorkoutSessionRepository
{
    public WorkoutSessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WorkoutSession>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(ws => ws.UserId == userId)
            .OrderByDescending(ws => ws.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(ws => ws.UserId == userId)
            .CountAsync(cancellationToken);
    }

    public async Task<WorkoutSession?> GetWithSetsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(ws => ws.SetHistories)
            .FirstOrDefaultAsync(ws => ws.Id == id, cancellationToken);
    }
}
