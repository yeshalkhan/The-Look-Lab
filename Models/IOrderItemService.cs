namespace The_Look_Lab.Models
{
    public interface IOrderItemService
    {
        public List<OrderItem> GetOrderItems(int orderId);
    }
}
