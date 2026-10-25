using Microsoft.EntityFrameworkCore;
using QuestLog.Application.Contracts.Repositories;
using QuestLog.Domain.Entities;
using QuestLog.Infrastructure.Persistence.Common;
using QuestLog.Infrastructure.Persistence.Contexts;

namespace QuestLog.Infrastructure.Persistence.Repositories;

public class GenreRepository(AppDbContext context) : Repository<Genre>(context), IGenreRepository
{
    public async Task<int> CountExistingAsync(IReadOnlyList<Guid> ids)
    {
        return await _dbSet.CountAsync(x => ids.Contains(x.Id));
    }

    public async Task<bool> HasGamesAsync(Guid id)
    {
        return await _context.Games.AnyAsync(g => g.Genres.Any(gg => gg.GenreId == id));
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        return await _dbSet.AnyAsync(x => x.Name == name && (excludeId == null || x.Id != excludeId));
    }
}
