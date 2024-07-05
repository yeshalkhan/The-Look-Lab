using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using The_Look_Lab.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace The_Look_Lab.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TheLookLabDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        private IRepository<Cart> _genericCartRepository;
        private ICartService _cartRepository;
        public CartController(UserManager<User> userManager, IRepository<Cart> genericCartRepository, ICartService cartRepository)
        {
            _userManager = userManager;
            _genericCartRepository = genericCartRepository;
            _cartRepository = cartRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // If user is authenticated / logged in, get cart data from database
            if (user != null)
            {
                string userId = user.Id;
                var productsQuantity = _cartRepository.GetAllProductsFromCart(userId);
                return View(new Tuple<List<Product>, List<int>>(productsQuantity.Item1,productsQuantity.Item2));
            }

            // Otherwise, get the data from session
            else
            {
                List<OrderItem> items = JsonConvert.DeserializeObject<List<OrderItem>>(HttpContext.Session.GetString("Cart") ?? "") ??
                    new List<OrderItem>();
                var productsQuantity = _cartRepository.GetAllProductsInOrder(items);
                return View(new Tuple<List<Product>, List<int>>(productsQuantity.Item1, productsQuantity.Item2));
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
                Cart cart = _cartRepository.GetItemFromCart(productId, userId);
                if (cart != null)
                    return RedirectToAction("Index", "Cart");

                cart = new Cart { UserId = userId, ProductId = productId, Quantity = quantity };
                _genericCartRepository.Add(cart);
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
                        return RedirectToAction("Index", "Cart");
                }
                OrderItem item = new OrderItem { ProductId = productId, Quantity = 1 };
                items.Add(item);
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(items));
            }

            return RedirectToAction("Index", "Cart");
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
                _cartRepository.UpdateCartQuantity(userId, productId, quantity);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("NotLoggedIn", "Home");
            }
        }

        //public async Task DeleteFromCart()
        //{
        //    // Get the currently logged-in user
        //    var user = await _userManager.GetUserAsync(User);

        //    // If user is authenticated / logged in, delete cart data from database
        //    if (user != null)
        //    {
        //        string userId = user.Id;
        //        CartRepository cartRepository = new CartRepository();
        //        cartRepository.DeleteFromCart(userId);
        //    }
        //    // Otherwise, delete the data from session
        //    else
        //    {
        //        HttpContext.Session.Remove("Cart");
        //    }
        //}
    }
}