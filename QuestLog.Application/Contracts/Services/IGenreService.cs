using QuestLog.Application.Common;
using QuestLog.Application.DTOs.Genres;

namespace QuestLog.Application.Contracts.Services;

public interface IGenreService
{
    Task<IReadOnlyList<GenreResponse>> GetAllAsync();
    Task<Result<GenreResponse>> GetByIdAsync(Guid id);
    Task<Result<GenreResponse>> CreateAsync(CreateGenreRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateGenreRequest request);
    Task<Result> DeleteAsync(Guid id);
}
