using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Domain.Entities;
using Domain.Interfaces;
using Application.Services;
using Microsoft.Extensions.Caching.Memory;

namespace The_Look_Lab.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
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

        public async Task<IActionResult> Index()
        {
            // Remove from cache after 30 minutes
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            // Check if count of products is already present in cache
            string cacheKey = "CountProducts";
            if (!_memoryCache.TryGetValue(cacheKey, out dynamic? countProducts))
            {
                TempData["CountProducts"] = await _productService.GetCount();
                _memoryCache.Set(cacheKey, TempData["CountProducts"], cacheEntryOptions);
            }
            else
                TempData["CountProducts"] = countProducts;

            // Check if count of orders is already present in cache
            cacheKey = "CountOrders";
            if (!_memoryCache.TryGetValue(cacheKey, out dynamic? countOrders))
            {
                TempData["CountOrders"] = await _orderService.GetCount();
                _memoryCache.Set(cacheKey, TempData["CountOrders"], cacheEntryOptions);
            }
            else
                TempData["CountOrders"] = countOrders;

            // Check if amount of monthly sales is already present in cache
            cacheKey = "MonthlySales";
            if (!_memoryCache.TryGetValue(cacheKey, out dynamic? monthlySales))
            {
                TempData["MonthlySales"] = await _orderService.GetTotalMonthlySales();
                _memoryCache.Set(cacheKey, TempData["MonthlySales"], cacheEntryOptions);
            }
            else
                TempData["MonthlySales"] = monthlySales;

            TempData["CountUsers"] = await _genericUserRepository.GetCount("AspNetUsers");

            return View();
        }

        public async Task<IActionResult> UsersTable()
        {
            IEnumerable<User> users = await _genericUserRepository.GetAll("AspNetUsers");
            return View(users.ToList());
        }

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
                return View(orders.ToList());
            }

            return View(allOrders?.ToList());
        }

        public async Task<IActionResult> ProductTable(int orderId)
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
                return View(productsWithQuantity);
            }

            return View(orderedProducts);
        }
    }
}
