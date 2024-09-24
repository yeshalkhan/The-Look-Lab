using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Domain.Entities;
using Application.Services;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderItemService _orderItemService;
        private readonly OrderService _orderService;
        private readonly CartService _cartService;
        private readonly ProductService _productService;
        private readonly IMemoryCache _memoryCache;
        public OrderController(OrderItemService orderItemService, OrderService orderService,
            CartService cartService, ProductService productService, IMemoryCache memoryCache)
        {
            _orderItemService = orderItemService;
            _orderService = orderService;
            _cartService = cartService;
            _productService = productService;
            _memoryCache = memoryCache;
        }

        // POST : api/order
        [HttpPost]
        public async Task<ActionResult> AddOrder([FromForm] Order _order)
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
                Status = "In progress"
            };

            int orderId = await _orderService.Add(order); // Returns the newly inserted Order ID

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
            }

            // Delete data from session
            HttpContext.Session.Remove("Cart");

            // Invalidate the cache so it shows the newly added order as well
            _memoryCache.Remove("AllOrders");
            _memoryCache.Remove("CountOrders");
            _memoryCache.Remove("MonthlySales");

            return Ok();

            //    Pass order details view model to the order confirmation page
            //    OrderDetails orderDetails = new OrderDetails { order = order, items = orderItems };
            //    var s = Newtonsoft.Json.JsonConvert.SerializeObject(orderDetails);
            //    TempData["NewOrderDetails"] = s;
            //    return RedirectToAction("OrderConfirmation", "Order");
        }

    }
}


