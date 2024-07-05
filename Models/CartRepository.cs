using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Dapper;
using static Dapper.SqlMapper;
namespace The_Look_Lab.Models
{
    public class CartRepository : ICartService
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TheLookLabDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public void UpdateCartQuantity(string userId, int productId, int quantity)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "UPDATE Cart SET Quantity = @q WHERE UserId = @u AND ProductId = @p";
            SqlParameter[] parameters = new SqlParameter[] {
             new SqlParameter("@u", userId),
             new SqlParameter("@p", productId),
             new SqlParameter("@q", quantity)
            };
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public (List<Product>,List<int>) GetAllProductsFromCart(string userId="")
        {
            List<Product> products = new List<Product>();
            List<int> quantites = new List<int>();
            SqlConnection connection = new SqlConnection(connectionString);
            if (userId != null)
            {
                string query = "SELECT P.Id,P.Name,P.Description,P.Price,P.Brand,P.Category,P.Image,C.Quantity FROM " +
                    "Product P JOIN Cart C ON P.Id = C.ProductId WHERE C.UserId = @u";
                SqlParameter parameter = new SqlParameter("u", userId);
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.Add(parameter);
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    Product p = new Product(int.Parse(sdr[0].ToString()), sdr[1].ToString(), sdr[2].ToString(), int.Parse(sdr[3].ToString()), sdr[4].ToString(), sdr[5].ToString(), sdr[6].ToString());
                    products.Add(p);
                    quantites.Add(int.Parse(sdr[7].ToString()));
                }
            }
            connection.Close();
            return (products, quantites);
        }

        public (List<Product>, List<int>) GetAllProductsInOrder(List<OrderItem> items)
        {
            List<Product> products = new List<Product>();
            List<int> quantites = new List<int>();
            string query;
            SqlConnection connection = new SqlConnection(connectionString);

            foreach (OrderItem item in items)
            {
                query = "SELECT * FROM Product WHERE Id = @Id";
                connection.Open();
                Product product = connection.QueryFirstOrDefault<Product>(query, new { Id = item.ProductId }) ?? new Product(); 
                products.Add(product);
                quantites.Add(item.Quantity);
                connection.Close();
            }
            return (products, quantites);
        }

        public Cart GetItemFromCart(int productId, string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM CART WHERE UserId = @UserId AND ProductId = @ProductId";
                connection.Open();
                Cart cart = connection.QueryFirstOrDefault<Cart>(query, new { ProductId = productId, UserId = userId }) ?? null;
                return cart;
            }
        }
        public void DeleteFromCart(string userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM CART WHERE UserId = @UserId";
                connection.Open();
                connection.QueryFirstOrDefault<Cart>(query, new { UserId = userId });
            }
        }
    }
}
