using QuestLog.Application.Common;
using QuestLog.Application.DTOs.Games;

namespace QuestLog.Application.Contracts.Services;

public interface IGameService
{
    Task<PagedResult<GameSummaryResponse>> GetPagedAsync(int page, int pageSize, string? search, Guid? genreId, Guid? platformId);
    Task<Result<GameDetailResponse>> GetBySlugAsync(string slug);
    Task<Result<GameDetailResponse>> CreateAsync(CreateGameRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateGameRequest request);
    Task<Result> DeleteAsync(Guid id);
}
