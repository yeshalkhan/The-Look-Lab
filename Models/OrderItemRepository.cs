using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using static Dapper.SqlMapper;
namespace The_Look_Lab.Models
{
    public class OrderItemRepository : IOrderItemService
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TheLookLabDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public List<OrderItem> GetOrderItems(int orderId)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                string query = $"SELECT * FROM [OrderItem] WHERE OrderId = @OrderId";
                connection.Open();
                return connection.Query<OrderItem>(query, new { OrderId = orderId }).ToList();
            }
        }

    }
}
