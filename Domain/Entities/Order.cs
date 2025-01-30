using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Order
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(80, ErrorMessage = "Customer name must not exceed 80 characters")]
        public string CustomerName { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid e-mail address")]
        public string? Email { get; set; }

        [Required]
        [MaxLength(11)]
        [MinLength(11, ErrorMessage = "Phone number must have 11 digits")]
        public string PhoneNumber { get; set; }

        [StringLength(50, ErrorMessage = "Postal code must not exceed 50 characters")]
        public string? PostalCode { get; set; }

        [Required]
        public string Address { get; set; }
        
        [StringLength(100, ErrorMessage = "State name must not exceed 100 characters")]
        public string? State { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }
        public string? DeliveryInstructions { get; set; }
        public int? TotalPrice { get; set; }
        public DateTime? OrderDate { get; set; }

        [StringLength(25)]
        public string? Status { get; set; }

        public string UserId { get; set; }
        public Order() { }
        public Order(string name = "", string email = "", string phoneNumber = "", string postalCode = "", string address = "", string state = "", string city = "", string deliveryInstructions = "", string userId = "")
        {
            CustomerName = name;
            Email = email;
            PhoneNumber = phoneNumber;
            PostalCode = postalCode;
            Address = address;
            State = state;
            City = city;
            DeliveryInstructions = deliveryInstructions;
            UserId = userId;
        }
    }
}
