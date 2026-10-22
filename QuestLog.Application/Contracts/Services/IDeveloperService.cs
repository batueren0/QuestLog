using QuestLog.Application.Common;
using QuestLog.Application.DTOs.Developers;

namespace QuestLog.Application.Contracts.Services;

public interface IDeveloperService
{
    Task<IReadOnlyList<DeveloperResponse>> GetAllAsync();
    Task<Result<DeveloperResponse>> GetByIdAsync(Guid id);
    Task<Result<DeveloperResponse>> CreateAsync(CreateDeveloperRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateDeveloperRequest request);
    Task<Result> DeleteAsync(Guid id);
}
