using Microsoft.EntityFrameworkCore;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;
using Weights.Infrastructure.Data;

namespace Weights.Infrastructure.Repositories;

public class WorkoutTemplateRepository(ApplicationDbContext context) : Repository<WorkoutTemplate, int>(context), IWorkoutTemplateRepository
{
    public async Task<IEnumerable<WorkoutTemplate>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(wt => wt.WorkoutTemplateExercises)
            .Where(wt => wt.UserId == userId && !wt.IsArchived)
            .ToListAsync(cancellationToken);
    }

    public async Task<WorkoutTemplate?> GetWithExercisesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(wt => wt.WorkoutTemplateExercises)
                .ThenInclude(wte => wte.Exercise)
            .FirstOrDefaultAsync(wt => wt.Id == id, cancellationToken);
    }
}
