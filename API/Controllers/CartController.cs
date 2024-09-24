using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Application.Services;
using Newtonsoft.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;
        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // GET : api/cart
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<OrderItem> items = JsonConvert.DeserializeObject<List<OrderItem>>(HttpContext.Session.GetString("Cart") ?? "") ??
                new List<OrderItem>();
            var productsWithQuantity = await _cartService.GetAllProductsFromSession(items);
            return new OkObjectResult(new Tuple<List<Product>, List<int>>(productsWithQuantity.Item1, productsWithQuantity.Item2));

        }

        // POST : api/cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            List<OrderItem> items = JsonConvert.DeserializeObject<List<OrderItem>>(HttpContext.Session.GetString("Cart") ?? "") ??
                   new List<OrderItem>();

            // Check if item already exists in cart
            foreach (OrderItem i in items)
            {
                if (i.ProductId == productId)
                {
                    // Update the quantity if item is already in cart
                    i.Quantity += quantity;
                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(items));
                    return Ok("Product added to cart successfully");
                }
            }

            OrderItem item = new OrderItem { ProductId = productId, Quantity = 1 };
            items.Add(item);
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(items));
            return Ok("Product added to cart successfully");
        }

        // DELETE: api/cart
        [HttpDelete]
        public async Task<IActionResult> DeleteFromCart(int productId)
        {
            List<OrderItem> items = JsonConvert.DeserializeObject<List<OrderItem>>(HttpContext.Session.GetString("Cart") ?? "") ??
                   new List<OrderItem>();

            for (int i = 0; i < items.Count; i++)
            {
                // Delete the product from list of items
                if (items[i].ProductId == productId)
                    items.RemoveAt(i);
            }

            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(items));
            return Ok();
        }
    }
}