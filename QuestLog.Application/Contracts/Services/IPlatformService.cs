using QuestLog.Application.Common;
using QuestLog.Application.DTOs.Platforms;

namespace QuestLog.Application.Contracts.Services;

public interface IPlatformService
{
    Task<IReadOnlyList<PlatformResponse>> GetAllAsync();
    Task<Result<PlatformResponse>> GetByIdAsync(Guid id);
    Task<Result<PlatformResponse>> CreateAsync(CreatePlatformRequest request);
    Task<Result> UpdateAsync(Guid id, UpdatePlatformRequest request);
    Task<Result> DeleteAsync(Guid id);
}
