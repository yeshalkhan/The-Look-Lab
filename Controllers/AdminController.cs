using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using The_Look_Lab.Models;

namespace The_Look_Lab.Controllers
{
    [Authorize (Policy ="AdminOnly")]
    public class AdminController : Controller
    {
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TheLookLabDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        private IRepository<Product> _genericProductRepository;
        private IRepository<Order> _genericOrderRepository;
        private IRepository<User> _genericUserRepository;
        private IOrderService _orderRepository;
        private ICartService _cartRepository;
        private IOrderItemService _orderItemRepository;
        public AdminController(IRepository<Product> genericProductRepository, IRepository<Order> genericOrderRepository, IRepository<User> genericUserRepository, IOrderService orderRepository, ICartService cartRepository, IOrderItemService orderItemRepository)
        {
            _genericProductRepository = genericProductRepository;
            _genericOrderRepository = genericOrderRepository;
            _genericUserRepository = genericUserRepository;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _orderItemRepository = orderItemRepository;
        }
        public IActionResult Index()
        {
            TempData["CountProducts"] = _genericProductRepository.GetCount();
            TempData["CountOrders"] = _genericOrderRepository.GetCount();
            TempData["CountUsers"] = _genericUserRepository.GetCount("AspNetUsers");
            TempData["MonthlySales"] = _orderRepository.GetTotalMonthlySales();
            return View();
        }

        public IActionResult UsersTable() 
        {
            List<User> users = _genericUserRepository.GetAll("AspNetUsers").ToList();
            return View(users);
        }
        public IActionResult OrdersTable()
        {
            List<Order> orders = _genericOrderRepository.GetAll().ToList();
            return View(orders);
        }
        public IActionResult ProductTable(int orderId)
        {
            List<OrderItem> orderItems = _orderItemRepository.GetOrderItems(orderId);
            var productsQuantity = _cartRepository.GetAllProductsInOrder(orderItems);
            return View(productsQuantity);
        }
    }
}
