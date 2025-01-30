using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Domain.Entities;
using Application.Services;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace The_Look_Lab.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<InventoryHub> _hubContext;
        private readonly OrderItemService _orderItemService;
        private readonly OrderService _orderService;
        private readonly CartService _cartService;
        private readonly ProductService _productService;
        private readonly IMemoryCache _memoryCache;
        public OrderController(UserManager<User> userManager, OrderItemService orderItemService, OrderService orderService,
            CartService cartService, ProductService productService, IHubContext<InventoryHub> hubContext, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _orderItemService = orderItemService;
            _orderService = orderService;
            _cartService = cartService;
            _productService = productService;
            _hubContext = hubContext;
            _memoryCache = memoryCache;
        }

        public IActionResult Index(string quantitiesString = "", string productsString = "", int total = 0)
        {
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(productsString) ?? new List<Product>();
            ViewBag.TotalPrice = total;
            List<int> quantities = JsonConvert.DeserializeObject<List<int>>(quantitiesString) ?? new List<int>();
            CheckoutViewModel checkoutViewModel = new CheckoutViewModel();
            checkoutViewModel.products = products;
            checkoutViewModel.quantities = quantities;
            checkoutViewModel.order = new Order();
            return View(checkoutViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrder(Order _order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Order order = new Order
            {
                CustomerName = _order.CustomerName,
                Email = _order.Email,
                PhoneNumber = _order.PhoneNumber,
                PostalCode = _order.PostalCode,
                Address = _order.Address,
                State = _order.State,
                City = _order.City,
                DeliveryInstructions = _order.DeliveryInstructions,
                TotalPrice = _order.TotalPrice,
                OrderDate = DateTime.Now,
                Status = "In progress",
            };

            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
                order.UserId = user.Id;

            int orderId = await _orderService.Add(order); // Returns the newly inserted Order ID


            // If user is authenticated / logged in, get cart data from database
            if (user != null)
            {
                string userId = user.Id;

                // Add order items for the latest order
                var productsWithQuantity = await _cartService.GetAllProductsFromCart(userId);
                OrderItem item;
                int index = 0;
                foreach (var product in productsWithQuantity.Item1)
                {
                    item = new OrderItem { OrderId = orderId, ProductId = (int)product.Id, Quantity = productsWithQuantity.Item2[index] };
                    await _orderItemService.Add(item);

                    // Update product's stock
                    product.Stock -= productsWithQuantity.Item2[index++];
                    await _productService.Update(product);

                    // Notify clients about the stock update
                    await _hubContext.Clients.All.SendAsync("ReceiveInventoryUpdate", product.Id, product.Stock);
                }

                // Delete data from cart
                await _cartService.DeleteById(userId);
            }

            // Otherwise, get the data from session
            else
            {
                // Add order items for the latest order
                List<OrderItem> items = JsonConvert.DeserializeObject<List<OrderItem>>(HttpContext.Session.GetString("Cart") ?? "") ??
                    new List<OrderItem>();

                foreach (var item in items)
                {
                    item.OrderId = orderId;
                    await _orderItemService.Add(item);

                    // Update product stock
                    Product updatedProduct = await _productService.GetById(item.ProductId);
                    updatedProduct.Stock -= item.Quantity;
                    await _productService.Update(updatedProduct);

                    // Notify clients about the stock update
                    await _hubContext.Clients.All.SendAsync("ReceiveInventoryUpdate", updatedProduct.Id, updatedProduct.Stock);
                }

                // Delete data from session
                HttpContext.Session.Remove("Cart");
            }

            // Invalidate the cache so it shows the newly added order as well
            _memoryCache.Remove("AllOrders");
            _memoryCache.Remove("CountOrders");
            _memoryCache.Remove("MonthlySales");

            TempData["Title"] = "Order Confirmation - The Look Lab";
            TempData["Message"] = "Your order has been confirmed. Thank you for shopping with us!";
            return Ok();

            //    Pass order details view model to the order confirmation page
            //    OrderDetails orderDetails = new OrderDetails { order = order, items = orderItems };
            //    var s = Newtonsoft.Json.JsonConvert.SerializeObject(orderDetails);
            //    TempData["NewOrderDetails"] = s;
            //    return RedirectToAction("OrderConfirmation", "Order");
        }

        public IActionResult OrderConfirmation()
        {
            if (TempData["NewOrderDetails"] is string s)
            {
                var orderDetails = JsonConvert.DeserializeObject<OrderDetails>(s);
                return View(orderDetails);
            }
            return View();
        }

    }
}


