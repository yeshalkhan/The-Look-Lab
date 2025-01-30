using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using static NuGet.Packaging.PackagingConstants;

namespace The_Look_Lab.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly IMemoryCache _memoryCache;
        private readonly OrderService _orderService;
        private readonly CartService _cartService;
        private readonly OrderItemService _orderItemService;

        public UserController(UserManager<User> userManager, UserService userService, IMemoryCache memoryCache, 
            OrderService orderService, CartService cartService, OrderItemService orderItemService)
        {
            _userManager = userManager;
            _userService = userService;
            _memoryCache = memoryCache;
            _orderService = orderService;
            _cartService = cartService;
            _orderItemService = orderItemService;
        }

        [Route("User/Profile")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(string name, string email)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                user.Name = name;
                user.Email = email;
                await _userService.Update(user);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                string cacheKey = "AllOrders";

                // Check if orders are already present in cache
                if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Order>? allOrders))
                {
                    IEnumerable<Order> orders = await _orderService.GetAll();

                    // Remove from cache after 30 minutes
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                    _memoryCache.Set(cacheKey, orders, cacheEntryOptions);
                    return View(orders.Where(o => o.UserId == user.Id).ToList());
                }

                return View(allOrders?.Where(o => o.UserId == user.Id).ToList());
            }

            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        public async Task<IActionResult> OrderedProducts(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
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
            return RedirectToAction("Login", "Account", new { area = "Identity" });

        }
    }
}
