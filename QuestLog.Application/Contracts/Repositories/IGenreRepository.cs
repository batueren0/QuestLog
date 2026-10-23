using QuestLog.Application.Common;
using QuestLog.Domain.Entities;

namespace QuestLog.Application.Contracts.Repositories;

public interface IGenreRepository : IRepository<Genre>
{
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
    Task<bool> HasGamesAsync(Guid id);
    Task<int> CountExistingAsync(IReadOnlyList<Guid> ids);

}
