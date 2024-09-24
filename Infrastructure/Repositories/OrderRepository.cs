using Dapper;
using Microsoft.Data.SqlClient;
using Domain.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using static Dapper.SqlMapper;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        private readonly IRepository<Order> _repository;

        public OrderRepository(IConfiguration configuration, IRepository<Order> repository)
        {
            _repository = repository;
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<int> Add(Order order)
        {
            var properties = typeof(Order).GetProperties()
                                             .Where(p => p.Name != "Id");
            var columnName = string.Join(",", properties.Select(p => $"[{p.Name}]"));
            var parameterNames = string.Join(",", properties.Select(p => $"@{p.Name}"));
            var query = $@" INSERT INTO [Order] ({columnName}) VALUES ({parameterNames}); SELECT CAST(SCOPE_IDENTITY() as int)";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QuerySingleAsync<int>(query, order);
            }
        }

        public async Task<int> Update(Order order)
        {
            return await _repository.Update(order);
        }

        public async Task<int> DeleteById(dynamic id)
        {
            return await _repository.DeleteById(id);
        }

        public async Task<Order> GetById(dynamic id)
        {
            return await _repository.GetById(id);
        }

        public async Task<IEnumerable<Order>> GetAll(string? tableName = null)
        {
            return await _repository.GetAll();
        }

        public async Task<int> GetCount(string? tablename = null)
        {
            return await _repository.GetCount();
        }

        public async Task<int> GetTotalMonthlySales()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = @"SELECT SUM(TotalPrice) FROM [Order] WHERE MONTH(OrderDate) = MONTH(GETDATE()) AND YEAR(OrderDate) = YEAR(GETDATE())";
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }
    }
}
