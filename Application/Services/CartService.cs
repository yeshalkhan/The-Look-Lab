using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class CartService
    {
        private readonly ICartRepository cartRepository;

        public CartService(ICartRepository _cartRepository)
        {
            cartRepository = _cartRepository;
        }

        public async Task<int> Add(Cart cart)
        {
            return await cartRepository.Add(cart);
        }

        public async Task<int> Update(Cart cart)
        {
            return await cartRepository.Update(cart);
        }

        public async Task<int> DeleteById(dynamic userId)
        {
            return await cartRepository.DeleteById(userId);
        }

        public async Task<Cart> GetById(dynamic id)
        {
            return await cartRepository.GetById(id);
        }

        public async Task<IEnumerable<Cart>> GetAll(string? tableName = null)
        {
            return await cartRepository.GetAll();
        }

        public async Task<int> GetCount(string? tablename = null)
        {
            return await cartRepository.GetCount();
        }

        public async Task<(List<Product>, List<int>)> GetAllProductsFromCart(string userId = "")
        {
            return await cartRepository.GetAllProductsFromCart(userId);
        }

        public async Task<(List<Product>, List<int>)> GetAllProductsFromSession(List<OrderItem> items)
        {
            return await cartRepository.GetAllProductsFromSession(items);
        }

        public async Task<Cart?> GetItemFromCart(int productId, string userId)
        {
            return await cartRepository.GetItemFromCart(productId, userId);
        }

        public async Task<int> DeleteFromCart(int productId, string userId)
        {
            return await cartRepository.DeleteFromCart(productId, userId);
        }
    }
}
