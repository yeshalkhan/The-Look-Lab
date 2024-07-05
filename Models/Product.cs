namespace The_Look_Lab.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public Product() { }
        public Product(int id = 0, string name = "", string description = "", int price = 0, string brand = "", string category = "", string image="")
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Brand = brand;
            Category = category;
            Image = image;
        }


    }
}
