using Domain.Entities;
namespace Domain.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        public Task<(List<Product>, List<int>)> GetAllProductsFromCart(string userId = "");
        public Task<(List<Product>, List<int>)> GetAllProductsFromSession(List<OrderItem> items);
        public Task<Cart?> GetItemFromCart(int productId, string userId);
        public Task<int> DeleteFromCart(int productId, string userId);
    }
}
