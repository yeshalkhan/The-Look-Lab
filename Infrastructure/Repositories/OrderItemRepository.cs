using Dapper;
using Microsoft.Data.SqlClient;
using static Dapper.SqlMapper;
using Domain.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        private readonly IRepository<OrderItem> _repository;

        public OrderItemRepository(IConfiguration configuration, IRepository<OrderItem> repository)
        {
            _repository = repository;
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<int> Add(OrderItem orderItem)
        {
            return await _repository.Add(orderItem);
        }

        public async Task<int> Update(OrderItem orderItem)
        {
            return await _repository.Update(orderItem);
        }

        public async Task<int> DeleteById(dynamic id)
        {
            return await _repository.DeleteById(id);
        }

        public async Task<OrderItem> GetById(dynamic id)
        {
            return await _repository.GetById(id);
        }

        public async Task<IEnumerable<OrderItem>> GetAll(string? tableName = null)
        {
            return await _repository.GetAll();
        }

        public async Task<int> GetCount(string? tablename = null)
        {
            return await _repository.GetCount();
        }

        public async Task<List<OrderItem>> GetOrderItems(int orderId)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                string query = $"SELECT * FROM [OrderItem] WHERE OrderId = @OrderId";
                await connection.OpenAsync();
                var results = await connection.QueryAsync<OrderItem>(query, new { OrderId = orderId });
                return results.ToList();
            }
        }

    }
}
