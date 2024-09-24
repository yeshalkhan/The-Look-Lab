using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class AddProductDto
    {
        public int? Id { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? Image { get; set; }
        public int Stock { get; set; }
    }
}
