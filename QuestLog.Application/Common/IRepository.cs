using QuestLog.Domain.Common;

namespace QuestLog.Application.Common;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    void Update(TEntity entity);
    Task AddAsync(TEntity entity);
    void Delete(TEntity entity);
}
