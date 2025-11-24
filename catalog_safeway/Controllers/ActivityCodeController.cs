using catalog_safeway.Data;
using catalog_safeway.Models;
using catalog_safeway.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace catalog_safeway.Controllers
{
    public class ActivityCodeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IProductServices _productServices;

        public ActivityCodeController(AppDbContext context, IWebHostEnvironment env, IProductServices productServices)
        {
            _context = context;
            _env = env;
            _productServices = productServices;
        }

        [HttpGet]
        public IActionResult EditProduct(string? code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var products = _context.Products.FirstOrDefault(w => w.Code == code);

                return View(products ?? new Product());
            }
            else
                return View();
        }



        //[HttpPost]
        //public async Task<IActionResult> EditProduct(Product model, IFormFile image)
        //{
        //    if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrEmpty(model.Code))
        //    {
        //        if (image != null)
        //        {
        //            string folder = Path.Combine(_env.WebRootPath, "images");
        //            Directory.CreateDirectory(folder);
        //            string rootFile = Path.Combine(folder, image.FileName);

        //            using (var stream = new FileStream(rootFile, FileMode.Create))
        //            {
        //                await image.CopyToAsync(stream);
        //            }
        //            model.ImagePath = "/images/" + image.FileName;
        //        }
        //        _context.Products.Update(model);
        //        await _context.SaveChangesAsync();
        //        ModelState.Clear();
        //        ViewBag.messageSucessfull = "Item upgraded";
        //        return View(model);
        //    }
        //    else
        //    {
        //        ViewBag.messageError = "Please fill out all required fields befor submitting.";
        //        return View(model);
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> EditProduct(Product model, IFormFile image)
        //{
        //    if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrEmpty(model.Code))
        //    {
        //        if (image != null)
        //        {
        //            using var ms = new MemoryStream();
        //            await image.CopyToAsync(ms);
        //            byte[] imageBytes = ms.ToArray();
        //            string base64String = Convert.ToBase64String(imageBytes);
        //            // Guardar la cadena en el modelo
        //            model.ImagePath = base64String;
        //        }

        //        _context.Products.Update(model);
        //        await _context.SaveChangesAsync();
        //        ModelState.Clear();
        //        ViewBag.messageSucessfull = "Item upgraded";
        //        return View(model);
        //    }
        //    else
        //    {
        //        ViewBag.messageError = "Please fill out all required fields before submitting.";
        //        return View(model);
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product model, IFormFile image)
        {
            if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrEmpty(model.Code))
            {
                if (image != null)
                {
                    using var ms = new MemoryStream();
                    await image.CopyToAsync(ms);
                    model.ImagePath = ms.ToArray(); // Guardar directamente como byte[]
                }

                _context.Products.Update(model);
                await _context.SaveChangesAsync();
                ModelState.Clear();
                ViewBag.messageSucessfull = "Item upgraded";
                return View(model);
            }
            else
            {
                ViewBag.messageError = "Please fill out all required fields before submitting.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ActivityCode()
        {
            var products = _context.Products.ToList();

            if (products != null)
            {
                var json = JsonSerializer.Serialize(products);
                HttpContext.Session.SetString("Product", json);
            }
            if (products.Any())
            {
                var selectedProduct = _productServices.getRandomProduct(products, true);
                return View(selectedProduct);
            }

            return View(new Product());
        }

        [HttpPost]
        public IActionResult ActivityCode(Product model, IFormFile image, string action)
        {
            var products = new List<Product>();
            // Session 
            var productJson = HttpContext.Session.GetString("Product");

            if (productJson != null)
            {
                products = JsonSerializer.Deserialize<List<Product>>(productJson);
            }
            else
                products = _context.Products.ToList();

            if (action == "clue")
            {
                Product? productUsed = products.Where(w => w.Id == model.Id).FirstOrDefault();
                ModelState.Clear();
                model.Code = productUsed.Code;
                model.ImagePath = productUsed.ImagePath;

                //if (!string.IsNullOrEmpty(model.Code) && productUsed.Count > 0)
                //{
                //    ModelState.Clear();
                //    var selectedProduct = _productServices.getRandomProduct(products, true);
                //    ViewBag.messageSucessfull = "Great";
                //    return View(selectedProduct);
                //}
                //else
                //{
                //    ViewBag.messageError = "The code is invalid. Please try again.";
                //    return View(model);
                //}
                return View(model);
            }

            if (action == "check")
            {
                var productUsed = products.Where(w => w.Code == model.Code && w.Description == model.Description).ToList();
                var productEntity = products.Where(w => w.Description == model.Description).FirstOrDefault();
                if (productUsed != null && !string.IsNullOrEmpty(model.Code) && productUsed.Count > 0)
                {
                    ModelState.Clear();
                    var selectedProduct = _productServices.getRandomProduct(products, true);
                    ViewBag.messageSucessfull = "Great";
                    return View(selectedProduct);
                }
                else
                {
                    model.ImagePath = productEntity.ImagePath;
                    ViewBag.messageError = "The code is invalid. Please try again.";
                    return View(model);
                }
            }

            if (action == "skip")
            {
                ModelState.Clear();
                var selectedProduct = _productServices.getRandomProduct(products, true);
                ViewBag.messageSucessfull = "";
                return View(selectedProduct);
            }
            return View(new Product());
        }
    }
}
