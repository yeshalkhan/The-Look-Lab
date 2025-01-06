using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Domain.Entities;
using Application.Services;
using Microsoft.Extensions.Caching.Memory;
using API.DTO;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ProductService _productService;
        private readonly IMemoryCache _memoryCache;

        public ProductsController(IWebHostEnvironment env, ProductService productService, IMemoryCache memoryCache)
        {
            _env = env;
            _productService = productService;
            _memoryCache = memoryCache;
        }

        // GET: api/products
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            string cacheKey = "AllProducts";

            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Product>? allProducts))
            {
                allProducts = await _productService.GetAll();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, allProducts, cacheEntryOptions);
            }

            return Ok(allProducts?.ToList());
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDto productDto, [FromForm] IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Image is required.");
            }

            string contentPath = _env.ContentRootPath;
            string path = Path.Combine(contentPath, "product images");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string uniqueFileName = $"{Guid.NewGuid().ToString().Substring(0, 8)}_{imageFile.FileName}";
            string imagePath = Path.Combine(path, uniqueFileName);
            using (FileStream fileStream = new FileStream(imagePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            var product = new Product
            {
                Name = productDto.ProductName??"",
                Price = productDto.Price,
                Description = productDto.Description,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Stock = productDto.Stock,
                Image = $"/product images/{uniqueFileName}"
            };

            await _productService.Add(product);

            // Invalidate the cache so it shows the newly added product as well
            _memoryCache.Remove("LatestProducts");
            _memoryCache.Remove("AllProducts");
            _memoryCache.Remove("CountProducts");
            _memoryCache.Remove(product.Category + "Products");


            return Ok();
        }

        // PUT: api/products
        [HttpPut]
        public async Task<IActionResult> EditProduct([FromForm] AddProductDto product, [FromForm] IFormFile imageFile)
        {
            string imagePath = product.Image ?? string.Empty;

            if (imageFile != null && imageFile.Length > 0)
            {
                string contentPath = _env.ContentRootPath;
                string path = Path.Combine(contentPath, "product images");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string uniqueFileName = $"{Guid.NewGuid().ToString()}_{imageFile.FileName}";
                imagePath = Path.Combine(path, uniqueFileName);
                using (FileStream fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                imagePath = $"/product images/{uniqueFileName}";
            }

            product.Image = imagePath;
            var updatedProduct = new Product
            {
                Id = product.Id,
                Name = product.ProductName ?? "",
                Price = product.Price,
                Description = product.Description,
                Brand = product.Brand,
                Category = product.Category,
                Stock = product.Stock,
                Image = product.Image
            };
            await _productService.Update(updatedProduct);

            // Invalidate the cache so it shows the modified product
            _memoryCache.Remove("LatestProducts");
            _memoryCache.Remove("AllProducts");
            _memoryCache.Remove("Product" + product.Id);
            _memoryCache.Remove(product.Category + "Products");

            return Ok();
        }

        // DELETE: api/products/{productId}
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var productToDelete = await _productService.GetById(productId);
            if (productToDelete == null)
            {
                return NotFound("No product found");
            }

            if (!string.IsNullOrEmpty(productToDelete.Image))
            {
                string contentPath = _env.ContentRootPath;
                string filePath = Path.Combine(contentPath, productToDelete.Image.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            await _productService.DeleteById(productId);

            // Invalidate the cache so it removes the deleted product
            _memoryCache.Remove("LatestProducts");
            _memoryCache.Remove("AllProducts");
            _memoryCache.Remove("CountProducts");
            _memoryCache.Remove("Product" + productId);
            _memoryCache.Remove(productToDelete.Category + "Products");


            return Ok("Product deleted successfully");
        }

        // GET: api/products/{productId}
        [AllowAnonymous]
        [HttpGet("{productId}")]
        public async Task<IActionResult> ViewProduct(int productId)
        {
            string cacheKey = "Product" + productId;

            if (!_memoryCache.TryGetValue(cacheKey, out Product? matchingProduct))
            {
                Product product = await _productService.GetById(productId);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, product, cacheEntryOptions);
                return new OkObjectResult(product);
            }
            return new OkObjectResult(matchingProduct);
        }

        // GET: api/products/search?searchterm={term}
        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchterm)
        {
            var products = await _productService.SearchProducts(searchterm);
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("Categories/{categoryName}")]
        public async Task<IActionResult> ViewCategory(string categoryName)
        {
            string cacheKey = categoryName + "Products";

            // Check if products are already present in cache 
            if (!_memoryCache.TryGetValue(cacheKey, out List<Product>? categoryProducts))
            {
                List<Product> products = await _productService.GetCategory(categoryName);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, products, cacheEntryOptions);
                return new OkObjectResult(products);
            }

            return new OkObjectResult(categoryProducts);
        }
    }
}
