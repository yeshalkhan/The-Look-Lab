using System.Collections.Generic;

namespace The_Look_Lab.Models
{
    public interface IRepository<TEntity>
    {
        public void Add(TEntity entity);
        public void Update(TEntity entity);
        public void DeleteById(int id);
        public TEntity GetById(int id);
        public IEnumerable<TEntity> GetAll(string tablename = null);
        public int GetCount(string tablename = null);

    }
}
