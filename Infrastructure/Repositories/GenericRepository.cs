using Microsoft.Data.SqlClient;
using Dapper;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity>
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        public GenericRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<int> Add(TEntity entity)
        {
            var tableName = typeof(TEntity).Name;
            var properties = typeof(TEntity).GetProperties()
                                             .Where(p => p.Name != "Id");
            var columnName = string.Join(",", properties.Select(p => $"[{p.Name}]"));
            var parameterNames = string.Join(",", properties.Select(p => $"@{p.Name}"));
            var query = $"INSERT INTO [{tableName}] ({columnName}) VALUES ({parameterNames})";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(query, entity);
            }
        }

        public async Task<int> DeleteById(dynamic id)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var query = $"DELETE FROM [{tableName}] WHERE [{primaryKey}] = @{primaryKey}";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(query, new { Id = id });
            }
        }

        public async Task<int> Update(TEntity entity)
        {
            var tableName = typeof(TEntity).Name == "User" ? "AspNetUsers" : typeof(TEntity).Name;
            var primaryKey = "Id";
            var properties = typeof(TEntity).GetProperties()
                                             .Where(p => p.Name != primaryKey);
            var setClause = string.Join(",", properties.Select(p => $"[{p.Name}] = @{p.Name}"));
            var query = $"UPDATE [{tableName}] SET {setClause} WHERE [{primaryKey}] = @{primaryKey}";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(query, entity);
            }
        }

        public async Task<IEnumerable<TEntity>> GetAll(string? tableName = null)
        {
            var table = tableName ?? typeof(TEntity).Name;
            var query = $"SELECT * FROM [{table}] ORDER BY Id DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<TEntity>(query);
            }
        }

        public async Task<TEntity> GetById(dynamic id)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var query = $"SELECT * FROM [{tableName}] WHERE [{primaryKey}] = @{primaryKey}";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<TEntity>(query, new { Id = id });
            }
        }

        public async Task<int> GetCount(string? tableName = null)
        {
            var table = tableName ?? typeof(TEntity).Name;
            var query = $"SELECT COUNT(*) FROM [{table}]";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }
    }
}
