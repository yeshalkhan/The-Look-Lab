using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Dapper;
using static Dapper.SqlMapper;
using Domain.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        private readonly IRepository<Cart> _repository;

        public CartRepository(IConfiguration configuration, IRepository<Cart> repository)
        {
            _repository = repository;
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<int> Add(Cart cart)
        {
            return await _repository.Add(cart);
        }

        public async Task<int> Update(Cart cart)
        {
            var query = $"UPDATE Cart SET Quantity = @Quantity WHERE UserId = @UserId AND ProductId = @ProductId";
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(query, cart);
            }
        }

        public async Task<int> DeleteById(dynamic userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM CART WHERE UserId = @UserId";
                await connection.OpenAsync();
                return await connection.ExecuteAsync(query, new { UserId = userId });
            }
        }

        public async Task<Cart> GetById(dynamic id)
        {
            return await _repository.GetById(id);
        }

        public async Task<IEnumerable<Cart>> GetAll(string? tableName = null)
        {
            return await _repository.GetAll();
        }

        public async Task<int> GetCount(string? tablename = null)
        {
            return await _repository.GetCount();
        }

        public async Task<(List<Product>, List<int>)> GetAllProductsFromCart(string userId = "")
        {
            List<Product> products = new List<Product>();
            List<int> quantities = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                if (!string.IsNullOrEmpty(userId))
                {
                    string query = @"SELECT P.Id, P.Name, P.Description, P.Price, P.Brand, P.Category, P.Image, P.Stock, C.Quantity
                                    FROM Product P JOIN Cart C ON P.Id = C.ProductId WHERE C.UserId = @UserId";
                    var result = await connection.QueryAsync(query, new { UserId = userId });
                    var resultList = result.ToList();
                    foreach (var item in resultList)
                    {
                        Product p = new Product(item.Id, item.Name, item.Description, item.Price, item.Brand, item.Category, item.Image, item.Stock);
                        products.Add(p);
                        quantities.Add(item.Quantity);
                    }
                }
            }

            return (products, quantities);
        }

        public async Task<(List<Product>, List<int>)> GetAllProductsFromSession(List<OrderItem> items)
        {
            List<Product> products = new List<Product>();
            List<int> quantites = new List<int>();
            string query;
            SqlConnection connection = new SqlConnection(connectionString);

            foreach (OrderItem item in items)
            {
                query = "SELECT * FROM Product WHERE Id = @Id";
                await connection.OpenAsync();
                Product product = await connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = item.ProductId }) ?? new Product(); 
                products.Add(product);
                quantites.Add(item.Quantity);
                connection.Close();
            }
            return (products, quantites);
        }

        public async Task<Cart?> GetItemFromCart(int productId, string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM CART WHERE UserId = @UserId AND ProductId = @ProductId";
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<Cart>(query, new { ProductId = productId, UserId = userId });
            }
        }

        public async Task<int> DeleteFromCart(int productId, string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM CART WHERE UserId = @UserId AND ProductId = @ProductId";
                await connection.OpenAsync();
                return await connection.ExecuteAsync(query, new { UserId = userId, ProductId = productId });
            }
        }
    }
}
