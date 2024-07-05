using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace The_Look_Lab.Models
{
    public class GenericRepository<TEntity> : IRepository<TEntity>
    {
        private readonly string connectionString;

        public GenericRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Add(TEntity entity)
        {
            var tableName = typeof(TEntity).Name;
            var properties = typeof(TEntity).GetProperties()
                                             .Where(p => p.Name != "Id");
            var columnName = string.Join(",", properties.Select(p => $"[{p.Name}]"));
            var parameterNames = string.Join(",", properties.Select(p => $"@{p.Name}"));
            var query = $"INSERT INTO [{tableName}] ({columnName}) VALUES ({parameterNames})";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute(query, entity);
            }
        }

        public void DeleteById(int id)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var query = $"DELETE FROM [{tableName}] WHERE [{primaryKey}] = @{primaryKey}";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute(query, new { Id = id });
            }
        }

        public void Update(TEntity entity)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var properties = typeof(TEntity).GetProperties()
                                             .Where(p => p.Name != primaryKey);
            var setClause = string.Join(",", properties.Select(p => $"[{p.Name}] = @{p.Name}"));
            var query = $"UPDATE [{tableName}] SET {setClause} WHERE [{primaryKey}] = @{primaryKey}";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute(query, entity);
            }
        }

        public IEnumerable<TEntity> GetAll(string tablename = null)
        {
            var tableName = tablename ?? typeof(TEntity).Name;
            var query = $"SELECT * FROM [{tableName}]";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<TEntity>(query);
            }
        }

        public TEntity GetById(int id)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var query = $"SELECT * FROM [{tableName}] WHERE [{primaryKey}] = @{primaryKey}";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<TEntity>(query, new { Id = id });
            }
        }

        public int GetCount(string tablename = null)
        {
            var tableName = tablename ?? typeof(TEntity).Name;
            var query = $"SELECT COUNT(*) FROM [{tableName}]";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return connection.ExecuteScalar<int>(query);
            }
        }
    }
}
