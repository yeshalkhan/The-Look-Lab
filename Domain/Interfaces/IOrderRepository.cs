using Domain.Entities;
namespace Domain.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        public Task<int> GetTotalMonthlySales();

    }
}
