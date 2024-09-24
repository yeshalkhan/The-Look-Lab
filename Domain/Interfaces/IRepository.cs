using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IRepository<TEntity>
    {
        public Task<int> Add(TEntity entity);
        public Task<int> Update(TEntity entity);
        public Task<int> DeleteById(dynamic id);
        public Task<TEntity> GetById(dynamic id);
        public Task<IEnumerable<TEntity>> GetAll(string? tablename = null);
        public Task<int> GetCount(string? tablename = null);

    }
}
