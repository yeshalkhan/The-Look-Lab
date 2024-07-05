using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using The_Look_Lab.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using The_Look_Lab.Data;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using NuGet.Packaging.Signing;


namespace The_Look_Lab.Controllers
{
    [Authorize(Policy = "AdminOnly")]

    public class ProductsController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TheLookLabDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public ProductsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            IRepository<Product> repo = new GenericRepository<Product>(connectionString);
            List<Product> products = repo.GetAll().ToList();
            ViewBag.Heading = "PRODUCTS";
            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(string name, string description, int price, string brand, string category, IFormFile image)
        {
            // save image in folder
            string wwwRootPath = _env.WebRootPath;
            string path = Path.Combine(wwwRootPath, "product images");
            string imagePath, uniqueFileName = "";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (image.Length > 0)
            {
                string uniqueIdentifier = Guid.NewGuid().ToString().Substring(0, 8); // Take the first 8 characters of the GUID
                uniqueFileName = $"{uniqueIdentifier}_{image.FileName}";
                imagePath = Path.Combine(path, uniqueFileName);
                using FileStream fileStream = new FileStream(imagePath, FileMode.Create);
                image.CopyTo(fileStream);
            }

            imagePath = image.Length > 0 ? "/product images/" + uniqueFileName : "";
            Product p = new Product { Name = name, Description = description, Price = price, Brand = brand, Category = category, Image = imagePath };
            IRepository<Product> repo = new GenericRepository<Product>(connectionString);
            repo.Add(p);
            return RedirectToAction("ManageProducts", "Products");
        }

        [HttpGet]
        public IActionResult ManageProducts()
        {
            IRepository<Product> repo = new GenericRepository<Product>(connectionString);
            List<Product> products = repo.GetAll().ToList();
            ViewBag.Heading = "MANAGE PRODUCTS";
            return View(products);
        }


        [HttpPost]
        public IActionResult EditProductForm(string productString)
        {
            Product product = JsonSerializer.Deserialize<Product>(productString) ?? new Product();
            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(string name, string description, int price, string brand, string category, IFormFile image, string productString) 
        {
            // save image in folder
            Product product = JsonSerializer.Deserialize<Product>(productString) ?? new Product();
            string imagePath = product.Image, uniqueFileName = "";
            if (image != null)
            { 
                string wwwRootPath = _env.WebRootPath;
                string path = Path.Combine(wwwRootPath, "product images");
               
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (image.Length > 0)
                {
                    string uniqueIdentifier = Guid.NewGuid().ToString().Substring(0, 8); // Take the first 8 characters of the GUID
                    uniqueFileName = $"{uniqueIdentifier}_{image.FileName}";
                    imagePath = Path.Combine(path, uniqueFileName);
                    using FileStream fileStream = new FileStream(imagePath, FileMode.Create);
                    image.CopyTo(fileStream);
                    imagePath = "/product images/" + uniqueFileName;
                }
            }

            Product newProduct = new Product
            {
                Id = product.Id,
                Name = name ?? product.Name,
                Description = description ?? product.Description,
                Price = price == 0 ? product.Price : price,
                Brand = brand ?? product.Brand,
                Category = category ?? product.Category,
                Image = imagePath
            };
            IRepository<Product> repo=new GenericRepository<Product>(connectionString);
            repo.Update(newProduct);
            return RedirectToAction("ManageProducts", "Products");

        }

        [HttpPost]
        public IActionResult DeleteProduct(int productId)
        {
            IRepository<Product> repo=new GenericRepository<Product>(connectionString);
            Product productToDelete = repo.GetById(productId);
            if (productToDelete != null)
            {
                string imagePath = productToDelete.Image;
                repo.DeleteById(productId);

                // If the image path exists, delete the corresponding image file
                if (!string.IsNullOrEmpty(imagePath))
                {
                    string wwwRootPath = _env.WebRootPath;
                    string filePath = Path.Combine(wwwRootPath, imagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
            }
            return RedirectToAction("ManageProducts", "Products");
        }
        //[HttpGet]
        //public IActionResult ViewProduct()
        //{
        //
        //    return View();
        //}

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ViewProduct(int productId)
        {
            IRepository<Product> repo=new GenericRepository<Product>(connectionString);
            Product product = repo.GetById(productId);
            return View(product);
        }

    }
}