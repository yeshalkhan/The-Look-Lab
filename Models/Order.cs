namespace The_Look_Lab.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string DeliveryInstructions { get; set; }
        public int TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public Order() { }
        public Order( string name="", string email= "", string phoneNumber = "", string postalCode = "", string address = "", string state = "", string city = "", string deliveryInstructions = "")
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            PostalCode = postalCode;
            Address = address;
            State = state;
            City = city;
            DeliveryInstructions = deliveryInstructions;
        }
    }
}
