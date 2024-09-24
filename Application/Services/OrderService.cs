using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository orderRepository;

        public OrderService(IOrderRepository _orderRepository)
        {
            orderRepository = _orderRepository;
        }

        public async Task<int> Add(Order order)
        {
            return await orderRepository.Add(order);
        }

        public async Task<int> Update(Order order)
        {
            return await orderRepository.Update(order);
        }

        public async Task<int> DeleteById(dynamic id)
        {
            return await orderRepository.DeleteById(id);
        }

        public async Task<Order> GetById(dynamic id)
        {
            return await orderRepository.GetById(id);
        }

        public async Task<IEnumerable<Order>> GetAll(string? tableName = null)
        {
            return await orderRepository.GetAll();
        }

        public async Task<int> GetCount(string? tablename = null)
        {
            return await orderRepository.GetCount();
        }

        public async Task<int> GetTotalMonthlySales()
        {
            return await orderRepository.GetTotalMonthlySales();
        }
    }
}
