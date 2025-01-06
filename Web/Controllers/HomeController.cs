using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Domain.Entities;
using Application.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using System.Net;

namespace The_Look_Lab.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _memoryCache;
        private readonly ProductService _productService;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, ProductService productService, IMemoryCache memoryCache)
        {
            _logger = logger;
            _env = env;
            _productService = productService;
            _memoryCache = memoryCache;
        }

        // Apply Output Caching for 60 seconds
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<ActionResult> Index()
        {
            string cacheKey = "LatestProducts";
            ViewBag.Heading = "LATEST PRODUCTS";

            // Check if products are already present in cache 
            if (!_memoryCache.TryGetValue(cacheKey, out List<Product>? latestProducts))
            {
                List<Product> products = await _productService.GetLatestProducts();

                // Remove from cache after 15 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(cacheKey, products, cacheEntryOptions);
                return View(products);
            }
            return View(latestProducts);
        }

        // Apply Output Caching for 60 seconds
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string email, string name, string city, string message)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(email);
                    mail.To.Add("thelooklabcosmetics@gmail.com");
                    mail.Subject = $"Message from {email}";
                    mail.Body = $"{name} from {city} says:\n{message}";
                    mail.IsBodyHtml = false;

                    using (var smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential("thelooklabcosmetics@gmail.com", "uayf ycxo oupy yfqx");
                        await smtp.SendMailAsync(mail);
                    }
                }
                TempData["Message"] = "Thank you for your feedback!";
                return RedirectToAction("DisplayMessage");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Your feedback could not be sent. Please try again.";
                return RedirectToAction("DisplayMessage");
            }
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