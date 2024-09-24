using Domain.Entities;
namespace Domain.Interfaces
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        public Task<List<OrderItem>> GetOrderItems(int orderId);
    }
}
