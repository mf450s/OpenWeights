using Microsoft.EntityFrameworkCore;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;
using Weights.Infrastructure.Data;

namespace Weights.Infrastructure.Repositories;

public class MuscleRepository(ApplicationDbContext context) : Repository<Muscle, int>(context), IMuscleRepository
{
    public async Task<IEnumerable<Muscle>> GetByBodyPartAsync(string bodyPart, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.BodyPart == bodyPart)
            .ToListAsync(cancellationToken);
    }

    // public async Task<Muscle> DeleteAsync(Muscle muscle, CancellationToken cancellationToken = default)
    // {
    //     DbSet.Remove(muscle);
    //     await Context.SaveChangesAsync(cancellationToken);
    //     return muscle;
    // }
}
