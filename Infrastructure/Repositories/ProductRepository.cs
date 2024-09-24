using Dapper;
using Microsoft.Data.SqlClient;
using static Dapper.SqlMapper;
using Domain.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        private readonly IRepository<Product> _repository;

        public ProductRepository(IConfiguration configuration, IRepository<Product> repository)
        {
            _repository = repository;
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<int> Add(Product product)
        {
            return await _repository.Add(product);
        }

        public async Task<int> Update(Product product)
        {
            return await _repository.Update(product);
        }

        public async Task<int> DeleteById(dynamic id)
        {
            return await _repository.DeleteById(id);
        }

        public async Task<Product> GetById(dynamic id)
        {
            return await _repository.GetById(id);
        }

        public async Task<IEnumerable<Product>> GetAll(string? tableName = null)
        {
            return await _repository.GetAll();
        }

        public async Task<int> GetCount(string? tablename = null)
        {
            return await _repository.GetCount();
        }

        public async Task<List<Product>> GetLatestProducts()
        {
            var query = $"SELECT TOP 6 * FROM [Product] ORDER BY Id DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.QueryAsync<Product>(query);
                return result.ToList();
            }
        }

		public async Task<List<Product>> SearchProducts(string searchterm)
        {
            if (string.IsNullOrEmpty(searchterm))
                return new List<Product>();
            var query = $"SELECT * FROM PRODUCT WHERE LOWER([Name]) LIKE @searchterm OR LOWER([Category]) LIKE @searchterm OR LOWER([BRAND]) LIKE @searchterm";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                // Add % to the search term for partial matching
                string searchPattern = "%" + searchterm.ToLower() + "%";
                var result = await connection.QueryAsync<Product>(query, new { searchterm = searchPattern });
                return result.ToList();
            }
        }
	}
}
