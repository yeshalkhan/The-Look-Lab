using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Product
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(80, ErrorMessage = "Product name must not exceed 80 characters")]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero")]
        public int Price { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? Image { get; set; }
        //public IFormFile ImageFile { get; set; }

        [Required]
        public int Stock {  get; set; }
        public Product() { }
        public Product(int id = 0, string name = "", string description = "", int price = 0, string brand = "", string category = "", string image="", int stock = 0)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Brand = brand;
            Category = category;
            Image = image;
            Stock = stock;
        }


    }
}
