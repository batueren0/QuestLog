using Microsoft.EntityFrameworkCore;
using QuestLog.Application.Common;
using QuestLog.Application.Contracts.Repositories;
using QuestLog.Domain.Entities;
using QuestLog.Infrastructure.Persistence.Common;
using QuestLog.Infrastructure.Persistence.Contexts;

namespace QuestLog.Infrastructure.Persistence.Repositories;

public class ReviewRepository(AppDbContext context) : Repository<Review>(context), IReviewRepository
{
    public async Task<bool> ExistsAsync(Guid gameId, Guid userId)
    {
        return await _dbSet.AnyAsync(x => x.GameId == gameId && x.UserId == userId);
    }

    public async Task<PagedResult<Review>> GetPagedByGameIdAsync(Guid gameId, int page, int pageSize)
    {
        var query = _dbSet.AsNoTracking();
        query = query.Where(x => x.GameId == gameId);
        
        var totalCount = await query.CountAsync();

        var items = await query.Include(r => r.User)
            .OrderByDescending(x => x.CreatedOn)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Review>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
            
        };
            
    }
}
