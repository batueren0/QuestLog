using QuestLog.Application.Common;
using QuestLog.Domain.Entities;

namespace QuestLog.Application.Contracts.Repositories;

public interface IPlatformRepository : IRepository<Platform>
{
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
    Task<bool> HasGamesAsync(Guid id);
}
