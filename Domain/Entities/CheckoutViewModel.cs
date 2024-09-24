namespace Domain.Entities
{
    public class CheckoutViewModel
    {
        public Order? order { get; set; }
        public List<Product>? products { get; set; }
        public List<int>? quantities { get; set; }
    }

}
