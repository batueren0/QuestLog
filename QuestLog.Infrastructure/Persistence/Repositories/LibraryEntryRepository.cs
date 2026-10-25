using Microsoft.EntityFrameworkCore;
using QuestLog.Application.Contracts.Repositories;
using QuestLog.Domain.Entities;
using QuestLog.Domain.Enums;
using QuestLog.Infrastructure.Persistence.Common;
using QuestLog.Infrastructure.Persistence.Contexts;

namespace QuestLog.Infrastructure.Persistence.Repositories;

public class LibraryEntryRepository(AppDbContext context) : Repository<LibraryEntry>(context), ILibraryEntryRepository
{
    public async Task<LibraryEntry?> GetByUserAndGameAsync(Guid userId, Guid gameId)
    {
        return await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId && x.GameId == gameId);
    }

    public async Task<IEnumerable<LibraryEntry>> GetByUserIdAsync(Guid userId, GameStatus? status)
    {
        var query = _dbSet.AsNoTracking()
            .Where(x => x.UserId == userId);
        if (status.HasValue)
            query = query.Where(x => x.Status == status);

        return await query.Include(e => e.Game)
            .OrderByDescending(x => x.LastModifiedOn)
            .ToListAsync();

    }
}
