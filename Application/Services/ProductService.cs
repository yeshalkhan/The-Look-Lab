using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProductService
    {

        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository _productRepository)
        {
            productRepository = _productRepository;
        }

        public async Task<int> Add(Product product)
        {
            return await productRepository.Add(product);
        }

        public async Task<int> Update(Product product)
        {
            return await productRepository.Update(product);
        }

        public async Task<int> DeleteById(dynamic id)
        {
            return await productRepository.DeleteById(id);
        }

        public async Task<Product> GetById(dynamic id)
        {
            return await productRepository.GetById(id);
        }

        public async Task<IEnumerable<Product>> GetAll(string? tableName = null)
        {
            return await productRepository.GetAll();
        }

        public async Task<int> GetCount(string? tablename = null)
        {
            return await productRepository.GetCount();
        }

        public async Task<List<Product>> GetLatestProducts()
        {
            return await productRepository.GetLatestProducts();
        }

        public async Task<List<Product>> SearchProducts(string searchterm)
        {
            return await productRepository.SearchProducts(searchterm);
        }
    }
}
