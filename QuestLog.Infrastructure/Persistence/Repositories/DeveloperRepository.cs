using Microsoft.EntityFrameworkCore;
using QuestLog.Application.Contracts.Repositories;
using QuestLog.Domain.Entities;
using QuestLog.Infrastructure.Persistence.Common;
using QuestLog.Infrastructure.Persistence.Contexts;

namespace QuestLog.Infrastructure.Persistence.Repositories;

public class DeveloperRepository(AppDbContext context) : Repository<Developer>(context), IDeveloperRepository
{
    public async Task<bool> HasGamesAsync(Guid id)
    {
        return await _context.Games.AnyAsync(g => g.DeveloperId == id);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        return await _dbSet.AnyAsync(x => x.Name == name && (excludeId == null || x.Id != excludeId));
    }
}
