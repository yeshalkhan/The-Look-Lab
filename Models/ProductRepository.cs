using Dapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using static Dapper.SqlMapper;
namespace The_Look_Lab.Models
{
    public class ProductRepository 
    {
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TheLookLabDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public IEnumerable<Product> GetLatestProducts()
        {
            var query = $"SELECT TOP 6 * FROM [Product] ORDER BY Id DESC";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return connection.Query<Product>(query);
            }
        }

    }
}
