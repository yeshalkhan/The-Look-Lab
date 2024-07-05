namespace The_Look_Lab.Models
{
    public class Cart
    {
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
