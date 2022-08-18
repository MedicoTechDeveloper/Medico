using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medico.Domain.Interfaces
{
    public interface IViewRepository<out TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
    }

    public interface IRepository<TEntity>
        : IViewRepository<TEntity>, IDisposable where TEntity : class
    {
        Task AddAsync(TEntity obj);

        void AddRange(IEnumerable<TEntity> entities);

        void Update(TEntity obj);

        Task<int> SaveChangesAsync();
    }

    public interface IDeletableByIdRepository<TEntity>
        : IRepository<TEntity> where TEntity : class
    {
        void Remove(Guid id);
    }

    public interface IDeletableByEntityRepository<TEntity>
        : IRepository<TEntity> where TEntity : class
    {
        void Remove(TEntity entity);
    }
}
