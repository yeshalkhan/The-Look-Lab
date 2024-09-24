
namespace Domain.Entities
{
    public class OrderDetails
    {
        public Order? order {  get; set; }
        public List<OrderItem>? items { get; set; }
    }
}
