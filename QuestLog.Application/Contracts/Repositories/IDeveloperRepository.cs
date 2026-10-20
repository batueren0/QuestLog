using QuestLog.Application.Common;
using QuestLog.Domain.Entities;

namespace QuestLog.Application.Contracts.Repositories;

public interface IDeveloperRepository : IRepository<Developer>
{
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
    Task<bool> HasGamesAsync(Guid id);
}
