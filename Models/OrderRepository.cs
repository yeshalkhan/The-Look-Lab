using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Plugins;

namespace The_Look_Lab.Models
{
    public class OrderRepository
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TheLookLabDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public int GetLatestOrderId() 
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1 Id FROM [Order] ORDER BY Id DESC";
                return connection.QueryFirstOrDefault<int>(query);
            }
        }

        public int GetTotalMonthlySales()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"SELECT SUM(TotalPrice) FROM [Order] WHERE MONTH(OrderDate) = MONTH(GETDATE()) AND YEAR(OrderDate) = YEAR(GETDATE())";
                return connection.ExecuteScalar<int>(query);
            }
        }
    }
}
