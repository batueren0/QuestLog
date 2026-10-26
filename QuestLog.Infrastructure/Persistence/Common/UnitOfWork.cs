using QuestLog.Application.Common;
using QuestLog.Application.Contracts.Repositories;
using QuestLog.Infrastructure.Persistence.Contexts;
using QuestLog.Infrastructure.Persistence.Repositories;

namespace QuestLog.Infrastructure.Persistence.Common;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IGameRepository Games { get; }

    public IDeveloperRepository Developers { get; }

    public IGenreRepository Genres { get; }

    public ILibraryEntryRepository LibraryEntries { get; }

    public IPlatformRepository Platforms { get; }

    public IReviewRepository Reviews { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Games = new GameRepository(_context);
        Developers = new DeveloperRepository(_context);
        Genres = new GenreRepository(_context);
        LibraryEntries = new LibraryEntryRepository(_context);
        Platforms = new PlatformRepository(_context);
        Reviews = new ReviewRepository(_context);

    }

    public async ValueTask DisposeAsync() => await _context.DisposeAsync();

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
