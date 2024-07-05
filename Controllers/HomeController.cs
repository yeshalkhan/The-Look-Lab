using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using The_Look_Lab.Models;
using Microsoft.AspNetCore.Identity;

namespace The_Look_Lab.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private IProductService _productRepository;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, IProductService productRepository)
        {
            _logger = logger;
            _env = env;
            _productRepository = productRepository;
        }

        public ActionResult Index()
        {
            /* Cookie */
            //string data = string.Empty;
            //if (HttpContext.Request.Cookies.ContainsKey("FirstVisit"))
            //{
            //    data = "Welcome back to The Look Lab!\n Continue browsing";
            //}
            //else
            //{
            //    CookieOptions options = new CookieOptions { Expires = DateTime.Now.AddDays(1) };
            //    data = "Welcome to The Look Lab!";
            //    HttpContext.Response.Cookies.Append("FirstVisit", DateTime.Now.ToString(), options);
            //}
            //ViewBag.CookieData = data;
            /* Cookie */

            List<Product> products = _productRepository.GetLatestProducts();
            ViewBag.Heading = "LATEST PRODUCTS";
            return View(products);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public IActionResult DisplayMessage()
        {
            return View("_BasicPage");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}