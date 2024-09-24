using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class OrderItemService
    {
        private readonly IOrderItemRepository orderItemRepository;

        public OrderItemService(IOrderItemRepository _orderItemRepository)
        {
            orderItemRepository = _orderItemRepository;
        }

        public async Task<int> Add(OrderItem orderItem)
        {
            return await orderItemRepository.Add(orderItem);
        }

        public async Task<int> Update(OrderItem orderItem)
        {
            return await orderItemRepository.Update(orderItem);
        }

        public async Task<int> DeleteById(dynamic id)
        {
            return await orderItemRepository.DeleteById(id);
        }

        public async Task<OrderItem> GetById(dynamic id)
        {
            return await orderItemRepository.GetById(id);
        }

        public async Task<IEnumerable<OrderItem>> GetAll(string? tableName = null)
        {
            return await orderItemRepository.GetAll();
        }

        public async Task<int> GetCount(string? tablename = null)
        {
            return await orderItemRepository.GetCount();
        }

        public async Task<List<OrderItem>> GetOrderItems(int orderId)
        {
            return await orderItemRepository.GetOrderItems(orderId);
        }
    }
}
