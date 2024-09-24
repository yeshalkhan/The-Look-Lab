using Domain.Entities;
namespace Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        public Task<List<Product>> GetLatestProducts();
        public Task<List<Product>> SearchProducts(string query);
    }
}
