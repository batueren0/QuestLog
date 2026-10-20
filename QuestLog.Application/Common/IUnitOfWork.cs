using QuestLog.Application.Contracts.Repositories;

namespace QuestLog.Application.Common;

public interface IUnitOfWork : IAsyncDisposable
{
    IGameRepository Games { get; }
    IDeveloperRepository Developers { get; }
    IGenreRepository Genres { get; }
    ILibraryEntryRepository LibraryEntries { get; }
    IPlatformRepository Platforms { get; }
    IReviewRepository Reviews { get; }


    Task<int> SaveChangesAsync();
}
