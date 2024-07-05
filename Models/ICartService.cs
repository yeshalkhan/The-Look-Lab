using Microsoft.AspNetCore.Mvc;

namespace The_Look_Lab.Models
{
    public interface ICartService
    {
        public void UpdateCartQuantity(string userId, int productId, int quantity);
        public (List<Product>, List<int>) GetAllProductsFromCart(string userId = "");
        public (List<Product>, List<int>) GetAllProductsInOrder(List<OrderItem> items);
        public Cart GetItemFromCart(int productId, string userId);
        public void DeleteFromCart(string userId);

    }
}
