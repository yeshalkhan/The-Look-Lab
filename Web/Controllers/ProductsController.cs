using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Domain.Entities;
using Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.CodeAnalysis;

namespace The_Look_Lab.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ProductsController : Controller
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

        // Apply Output Caching for 60 seconds
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            string cacheKey = "AllProducts";
            ViewBag.Heading = "PRODUCTS";

            // Check if products are already present in cache 
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Product>? allProducts))
            {
                IEnumerable<Product> products = await _productService.GetAll();

                // Remove from cache after 30 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, products, cacheEntryOptions);
                return View(products.ToList());
            }
            return View(allProducts?.ToList());
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product _product, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                ModelState.AddModelError("image", "Image is required.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // save image in folder
            string wwwRootPath = _env.WebRootPath;
            string path = Path.Combine(wwwRootPath, "product images");
            string imagePath, uniqueFileName = "";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string uniqueIdentifier = Guid.NewGuid().ToString(); 
            uniqueFileName = $"{uniqueIdentifier}_{image.FileName}";
            imagePath = Path.Combine(path, uniqueFileName);
            using FileStream fileStream = new FileStream(imagePath, FileMode.Create);
            image.CopyTo(fileStream);

            imagePath = image.Length > 0 ? "/product images/" + uniqueFileName : "";
            Product product = new Product { Name = _product.Name, Description = _product.Description, Price = _product.Price,
                Brand = _product.Brand, Category = _product.Category, Image = imagePath, Stock = _product.Stock };
            await _productService.Add(product);

            // Invalidate the cache so it shows the newly added product as well
            _memoryCache.Remove("LatestProducts");
            _memoryCache.Remove("AllProducts");
            _memoryCache.Remove("CountProducts");
            _memoryCache.Remove(product.Category + "Products");

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ManageProducts()
        {
            string cacheKey = "AllProducts";
            ViewBag.Heading = "MANAGE PRODUCTS";

            // Check if products are already present in cache 
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Product>? allProducts))
            {
                IEnumerable<Product> products = await _productService.GetAll();

                // Remove from cache after 15 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, products, cacheEntryOptions);
                return View(products.ToList());
            }
            return View(allProducts?.ToList());
        }

        [HttpPost]
        public IActionResult EditProductForm(string productString)
        {
            Product product = JsonSerializer.Deserialize<Product>(productString) ?? new Product();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(string name, string description, int price, string brand, string category, IFormFile image, int stock, string productString) 
        {
            // save image in folder
            Product product = JsonSerializer.Deserialize<Product>(productString) ?? new Product();
            string imagePath = product.Image ?? string.Empty, uniqueFileName = "";
            if (image != null)
            { 
                string wwwRootPath = _env.WebRootPath;
                string path = Path.Combine(wwwRootPath, "product images");
               
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (image.Length > 0)
                {
                    string uniqueIdentifier = Guid.NewGuid().ToString().Substring(0, 8); // Take the first 8 characters of the GUID
                    uniqueFileName = $"{uniqueIdentifier}_{image.FileName}";
                    imagePath = Path.Combine(path, uniqueFileName);
                    using FileStream fileStream = new FileStream(imagePath, FileMode.Create);
                    image.CopyTo(fileStream);
                    imagePath = "/product images/" + uniqueFileName;
                }
            }

            Product newProduct = new Product
            {
                Id = product.Id,
                Name = name ?? product.Name,
                Description = description ?? product.Description,
                Price = price == 0 ? product.Price : price,
                Brand = brand ?? product.Brand,
                Category = category ?? product.Category,
                Image = imagePath,
                Stock = stock == 0 ? product.Stock : stock,
            };

            await _productService.Update(newProduct);

            // Invalidate the cache so it shows the modified product
            _memoryCache.Remove("LatestProducts");
            _memoryCache.Remove("AllProducts");
            _memoryCache.Remove("Product" + product.Id);
            _memoryCache.Remove(product.Category + "Products");

            return RedirectToAction("ManageProducts", "Products");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            Product productToDelete = await _productService.GetById(productId);
            if (productToDelete == null)
                return BadRequest("No product found");
            string imagePath = productToDelete.Image ?? string.Empty;
            if (await _productService.DeleteById(productId) > 0)
            {
                // If the image path exists, delete the corresponding image file
                if (!string.IsNullOrEmpty(imagePath))
                {
                    string wwwRootPath = _env.WebRootPath;
                    string filePath = Path.Combine(wwwRootPath, imagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                // Invalidate the cache so it removes the deleted product
                _memoryCache.Remove("LatestProducts");
                _memoryCache.Remove("AllProducts");
                _memoryCache.Remove("CountProducts");
                _memoryCache.Remove("Product" + productId);
                _memoryCache.Remove(productToDelete.Category + "Products");


                return Ok("Product deleted successfully");
            }
            else
                return BadRequest("Failed to delete product");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Search(string searchterm)
        {
            List<Product> products = await _productService.SearchProducts(searchterm);
            return PartialView("_ProductsPartial", new Tuple<List<Product>, string>(products, "Search Results"));
        }

        public async Task<IActionResult> AdminSearch(string searchterm)
        {
            List<Product> products = await _productService.SearchProducts(searchterm);
            ViewBag.Heading = "Search Results";
            return View("ManageProducts", products);
        }

        [AllowAnonymous]
        [Route("Products/ViewProduct/{productId}")]
        [HttpGet]
        public async Task<IActionResult> ViewProduct(int productId)
        {
            string cacheKey = "Product" + productId;

            // Check if products are already present in cache
            if (!_memoryCache.TryGetValue(cacheKey, out Product? matchingProduct))
            {
                Product product = await _productService.GetById(productId);

                // Remove from cache after 15 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, product, cacheEntryOptions);
                return View(product);
            }
            return View(matchingProduct);
        }

        // Apply Output Caching for 60 seconds
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        [AllowAnonymous]
        [Route("Products/Categories/{categoryName}")]
        public async Task<IActionResult> ViewCategory(string categoryName)
        {
            string cacheKey = categoryName + "Products";
            if (categoryName == "Lips") ViewBag.Heading = "LIP PRODUCTS";
            else if (categoryName == "Eyes") ViewBag.Heading = "EYE PRODUCTS";
            else ViewBag.Heading = "FACE PRODUCTS";

            // Check if products are already present in cache 
            if (!_memoryCache.TryGetValue(cacheKey, out List<Product>? categoryProducts))
            {
                List<Product> products = await _productService.GetCategory(categoryName);

                // Remove from cache after 15 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, products, cacheEntryOptions);
                return View("Index", products);
            }

            return View("Index", categoryProducts);
        }
    }
}