using QuestLog.Application.Common;
using QuestLog.Domain.Entities;
using QuestLog.Domain.Enums;

namespace QuestLog.Application.Contracts.Repositories;

public interface ILibraryEntryRepository : IRepository<LibraryEntry>
{
    Task<IEnumerable<LibraryEntry>> GetByUserIdAsync(Guid userId, GameStatus? status);
    Task<LibraryEntry?> GetByUserAndGameAsync(Guid userId, Guid gameId);
}
