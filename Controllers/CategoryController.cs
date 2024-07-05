using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace The_Look_Lab.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }
        
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult ManageCategories()
        {
            return View();
        }
    }
}
