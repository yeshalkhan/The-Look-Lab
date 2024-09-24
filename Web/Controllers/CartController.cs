using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Domain.Entities;
using Application.Services;

namespace The_Look_Lab.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly CartService _cartService;
        public CartController(UserManager<User> userManager, CartService cartService)
        {
            _userManager = userManager;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // If user is authenticated / logged in, get cart data from database
            if (user != null)
            {
                string userId = user.Id;
                var productsWithQuantity = await _cartService.GetAllProductsFromCart(userId);
                return View(new Tuple<List<Product>, List<int>>(productsWithQuantity.Item1, productsWithQuantity.Item2));
            }

            // Otherwise, get the data from session
            else
            {
                List<OrderItem> items = JsonConvert.DeserializeObject<List<OrderItem>>(HttpContext.Session.GetString("Cart") ?? "") ??
                    new List<OrderItem>();
                var productsWithQuantity = await _cartService.GetAllProductsFromSession(items);
                return View(new Tuple<List<Product>, List<int>>(productsWithQuantity.Item1, productsWithQuantity.Item2));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // If user is authenticated / logged in, store cart data in database
            if (user != null)
            {
                string userId = user.Id;

                // Check if item already exists in cart
                Cart cart = await _cartService.GetItemFromCart(productId, userId);
                if (cart != null)
                {
                    // Update the quantity if item is already in cart
                    cart.UserId = userId;
                    cart.Quantity += quantity;
                    if (await _cartService.Update(cart) > 0)
                        return Ok("Product added to cart successfully");
                }

                cart = new Cart { UserId = userId, ProductId = productId, Quantity = quantity };
                if (await _cartService.Add(cart) > 0)
                    return Ok("Product added to cart successfully");
            }

            // Otherwise, store the data in session
            else
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

                OrderItem item = new OrderItem { ProductId = productId, Quantity = quantity};
                items.Add(item);
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(items));
                return Ok("Product added to cart successfully");
            }

            // If we've reached here, something went wrong
            return BadRequest("Failed to add product to cart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // Check if user is authenticated
            if (user != null)
            {
                // User is logged in, so get the user ID
                string userId = user.Id;

                // Update the quantity in the cart
                await _cartService.Update(new Cart { UserId = userId, ProductId = productId, Quantity = quantity });
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("NotLoggedIn", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromCart(int productId)
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // If user is authenticated / logged in, delete product from database
            if (user != null)
            {
                string userId = user.Id;
                if (await _cartService.DeleteFromCart(productId, userId) == 1)
                    return Ok();
            }

            // Otherwise, delete the product from session
            else
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

            // If we've reached here, something went wrong
            return BadRequest("Failed to delete product from cart");
        }
    }
}