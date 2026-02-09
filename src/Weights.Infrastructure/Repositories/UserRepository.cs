using Microsoft.EntityFrameworkCore;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;
using Weights.Infrastructure.Data;

namespace Weights.Infrastructure.Repositories;

public class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}
