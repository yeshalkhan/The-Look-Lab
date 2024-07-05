using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using The_Look_Lab.Models;

namespace The_Look_Lab.Controllers
{
    [Authorize (Policy ="AdminOnly")]
    public class AdminController : Controller
    {
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TheLookLabDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public IActionResult Index()
        {
            IRepository<Product> productRepository = new GenericRepository<Product>(connectionString);
            TempData["CountProducts"] = productRepository.GetCount();
            IRepository<Order> orderRepository = new GenericRepository<Order>(connectionString);
            TempData["CountOrders"] = orderRepository.GetCount();
            IRepository<User> userRepository = new GenericRepository<User>(connectionString);
            TempData["CountUsers"] = userRepository.GetCount("AspNetUsers");
            OrderRepository repo = new OrderRepository();
            TempData["MonthlySales"] = repo.GetTotalMonthlySales();
            return View();
        }

        public IActionResult UsersTable() 
        {
            IRepository<User> repo = new GenericRepository<User>(connectionString);
            List<User> users = repo.GetAll("AspNetUsers").ToList();
            return View(users);
        }
        public IActionResult OrdersTable()
        {
            IRepository<Order> repo = new GenericRepository<Order>(connectionString);
            List<Order> orders = repo.GetAll().ToList();
            return View(orders);
        }
        public IActionResult ProductTable(int orderId)
        {
            OrderItemRepository orderItemRepository = new OrderItemRepository();
            List<OrderItem> orderItems = orderItemRepository.GetOrderItems(orderId);
            CartRepository cartRepository = new CartRepository();
            var productsQuantity = cartRepository.GetAllProductsInOrder(orderItems);
            return View(productsQuantity);
        }
    }
}
