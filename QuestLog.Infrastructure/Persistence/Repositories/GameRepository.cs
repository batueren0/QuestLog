using Microsoft.EntityFrameworkCore;
using QuestLog.Application.Common;
using QuestLog.Application.Contracts.Repositories;
using QuestLog.Domain.Entities;
using QuestLog.Infrastructure.Persistence.Common;
using QuestLog.Infrastructure.Persistence.Contexts;

namespace QuestLog.Infrastructure.Persistence.Repositories;

public class GameRepository(AppDbContext context) : Repository<Game>(context), IGameRepository
{
    public async Task<Game?> GetByIdWithRelationsAsync(Guid id)
    {
        return await _dbSet.Include(x => x.Genres)
            .Include(x => x.Platforms)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Game?> GetBySlugWithDetailsAsync(string slug)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Developer)
            .Include(x => x.Genres).ThenInclude(gg => gg.Genre)
            .Include(x => x.Platforms).ThenInclude(gp => gp.Platform)
            .SingleOrDefaultAsync(x => x.Slug == slug);
    }

    public async Task<PagedResult<Game>> GetPagedAsync(int page, int pageSize, string? search, Guid? genreId, Guid? platformId)
    {
        var query = _dbSet.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(g => g.Title.Contains(search));

        if (genreId.HasValue)
            query = query.Where(g => g.Genres.Any(gg => gg.GenreId == genreId));

        if (platformId.HasValue)
            query = query.Where(g => g.Platforms.Any(gp => gp.PlatformId == platformId));


        var totalCount = await query.CountAsync();

        var items = await query
            .Include(g => g.Developer)
            .Include(g => g.Genres).ThenInclude(gg => gg.Genre)
            .OrderByDescending(g => g.ReleaseDate)
            .ThenBy(g => g.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Game>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
    {
        return await _dbSet.AnyAsync(x => x.Slug == slug && (excludeId == null || x.Id != excludeId));
    }
}
