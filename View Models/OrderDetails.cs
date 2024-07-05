using The_Look_Lab.Models;

namespace The_Look_Lab.View_Models
{
    public class OrderDetails
    {
        public Order order {  get; set; }
        public List<OrderItem> items { get; set; }
    }
}
