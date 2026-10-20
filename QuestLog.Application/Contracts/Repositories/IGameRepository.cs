using QuestLog.Application.Common;
using QuestLog.Domain.Entities;

namespace QuestLog.Application.Contracts.Repositories;

public interface IGameRepository : IRepository<Game>
{
    Task<Game?> GetBySlugWithDetailsAsync(string slug);
    Task<PagedResult<Game>> GetPagedAsync(int page, int pageSize, string? search, Guid? genreId, Guid? platformId);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
}
