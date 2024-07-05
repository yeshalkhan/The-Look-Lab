using System.Text.Json.Serialization;

namespace The_Look_Lab.Models
{
    public class OrderItem
    {
        [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)] // Ignore if OrderId is 0 i.e., null
        public int? OrderId { get; set; }    // Make OrderId nullable to allow null values
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
