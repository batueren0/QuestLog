using QuestLog.Application.Common;
using QuestLog.Domain.Entities;

namespace QuestLog.Application.Contracts.Repositories;

public interface IReviewRepository : IRepository<Review>
{
    Task<PagedResult<Review>> GetPagedByGameIdAsync(Guid gameId, int page, int pageSize);
    Task<bool> ExistsAsync(Guid gameId, Guid userId);
}
