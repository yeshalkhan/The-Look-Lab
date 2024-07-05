using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Net;
using System.Security.Claims;
using The_Look_Lab.Models;
using The_Look_Lab.View_Models;

namespace The_Look_Lab.Controllers
{
    public class OrderController : Controller
    {
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TheLookLabDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        private readonly UserManager<User> _userManager;
        private IRepository<Order> _genericOrderRepository;
        private IRepository<OrderItem> _genericOrderItemRepository;
        private IOrderService _orderRepository;
        private ICartService _cartRepository;
        public OrderController(UserManager<User> userManager, IRepository<Order> genericOrderRepository, IRepository<OrderItem> genericOrderItemRepository, IOrderService orderRepository, ICartService cartRepository)
        {
            _userManager = userManager;
            _genericOrderRepository = genericOrderRepository;
            _genericOrderItemRepository = genericOrderItemRepository;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
        }

        public IActionResult Index(string quantitiesString = "", string productsString = "", int total = 0)
        {
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(productsString) ?? new List<Product>();
            ViewBag.TotalPrice = total;
            List<int> quantities = JsonConvert.DeserializeObject<List<int>>(quantitiesString) ?? new List<int>();
            return View(new Tuple<List<Product>, List<int>>(products, quantities));
        }

        [HttpPost]
        public async Task<ActionResult> AddOrder(string name, string email, string phoneNumber, string postalCode, string address, string state, string city, string deliveryInstructions, int totalPrice)
        {
            Order order = new Order
            {
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber,
                PostalCode = postalCode,
                Address = address,
                State = state,
                City = city,
                DeliveryInstructions = deliveryInstructions,
                TotalPrice = totalPrice,
                OrderDate = DateTime.Now,
                Status = "In Progress"
            };
            _genericOrderRepository.Add(order);

            // Get Id of the latest order
            int orderId = _orderRepository.GetLatestOrderId();

            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // If user is authenticated / logged in, get cart data from database
            if (user != null)
            {
                string userId = user.Id;

                // Add order items for the latest order
                var productsQuantity = _cartRepository.GetAllProductsFromCart(userId);
                OrderItem item;
                int index = 0;
                foreach (var product in productsQuantity.Item1)
                {
                    item = new OrderItem { OrderId = orderId, ProductId = product.Id, Quantity = productsQuantity.Item2[index++] };
                    _genericOrderItemRepository.Add(item);
                }

                // Delete data from cart
                _cartRepository.DeleteFromCart(userId);
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
                    _genericOrderItemRepository.Add(item);
                }

                // Delete data from session
                HttpContext.Session.Remove("Cart");
            }


            TempData["Title"] = "Order Confirmation - The Look Lab";
            TempData["Message"] = "Your order has been confirmed. Thank you for shopping with us!";
            return RedirectToAction("DisplayMessage", "Home", new { url = "OrderConfirmation" });

            //     Pass order details view model to the order confirmation page
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


