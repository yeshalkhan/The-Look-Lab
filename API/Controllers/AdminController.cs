using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using Domain.Interfaces;
using Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IRepository<User> _genericUserRepository;
        private readonly OrderService _orderService;
        private readonly CartService _cartService;
        private readonly OrderItemService _orderItemService;
        private readonly ProductService _productService;
        private readonly IMemoryCache _memoryCache;

        public AdminController(IRepository<User> genericUserRepository, OrderService orderService, CartService cartService,
            OrderItemService orderItemService, ProductService productService, IMemoryCache memoryCache)
        {
            _genericUserRepository = genericUserRepository;
            _orderService = orderService;
            _cartService = cartService;
            _orderItemService = orderItemService;
            _productService = productService;
            _memoryCache = memoryCache;
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> UsersTable()
        {
            string cacheKey = "AllUsers";

            // Check if orders are already present in cache
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Order>? allUsers))
            {
                IEnumerable<User> users = await _genericUserRepository.GetAll("AspNetUsers");

                // Remove from cache after 30 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, users, cacheEntryOptions);
                return new OkObjectResult(users.ToList());
            }

            return new OkObjectResult(allUsers?.ToList());
        }

        // GET: api/admin/orders
        [HttpGet("orders")]
        public async Task<IActionResult> OrdersTable()
        {
            string cacheKey = "AllOrders";

            // Check if orders are already present in cache
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Order>? allOrders))
            {
                IEnumerable<Order> orders = await _orderService.GetAll();

                // Remove from cache after 30 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, orders, cacheEntryOptions);
                return new OkObjectResult(orders.ToList());
            }

            return new OkObjectResult(allOrders?.ToList());
        }

        // GET: api/admin/products?orderId={id}
        [HttpGet("products")]
        public async Task<IActionResult> ProductTable([FromQuery] int orderId)
        {
            string cacheKey = "ProductsFor" + orderId;

            // Check if products are already present in cache
            if (!_memoryCache.TryGetValue(cacheKey, out (List<Product>, List<int>)? orderedProducts))
            {
                List<OrderItem> orderItems = await _orderItemService.GetOrderItems(orderId);
                var productsWithQuantity = await _cartService.GetAllProductsFromSession(orderItems);
              
                // Remove from cache after 30 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, productsWithQuantity, cacheEntryOptions);
                return new OkObjectResult(JsonConvert.SerializeObject(productsWithQuantity));
            }

            return new OkObjectResult(JsonConvert.SerializeObject(orderedProducts));
        }
    }
}
