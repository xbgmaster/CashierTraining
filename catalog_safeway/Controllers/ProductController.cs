using catalog_safeway.Data;
using catalog_safeway.Models;
using catalog_safeway.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using static System.Net.Mime.MediaTypeNames;

namespace catalog_safeway.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IProductServices _productServices;

        public ProductController(AppDbContext context, IWebHostEnvironment env, IProductServices productServices)
        {
            _context = context;
            _env = env;
            _productServices = productServices;
        }

        public IActionResult Index()
        {
            var products = _context.Products.OrderBy(s => s.Description).ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create(string? code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var products = _context.Products.FirstOrDefault(w => w.Code == code);
                return View(products ?? new Product());
            }
            else
                return View();
        }



        [HttpPost]
        public async Task<IActionResult> Create(Product model, IFormFile image, IFormFile excel, string action)
        {
            if (action == "save")
            {
                if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrEmpty(model.Code))
                {
                    var products = _context.Products.ToList();

                    if (products.Where(w => w.Code == model.Code).ToList().Count == 0)
                    {
                        if (image != null)
                        {
                            using var ms = new MemoryStream();
                            await image.CopyToAsync(ms);
                            byte[] imageBytes = ms.ToArray();
                            string base64String = Convert.ToBase64String(imageBytes);
                            // Guardar la cadena en el modelo
                            model.ImagePath = base64String;
                        }
                        model.Id = Guid.NewGuid().ToString();
                        _context.Products.Add(model);
                        await _context.SaveChangesAsync();
                        ModelState.Clear();
                        ViewBag.messageSucessfull = "Products has been successfully created.";
                        return View(new Product());
                    }
                    else
                        ViewBag.messageError = "This product code is already in use. Please enter a different code";
                    return View(model);

                }
                else
                {
                    ViewBag.messageError = "Please fill out all required fields befor submitting.";
                    return View(model);
                }
            }

            if (action == "upload")
            {
                if (excel != null && excel.Length > 0)
                {

                    using (var stream = new MemoryStream())
                    {
                        await excel.CopyToAsync(stream);
                        stream.Position = 0;

                        //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        //ExcelPackage.License = new EPPlusLicenseContext(LicenseContext.NonCommercial);

                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet != null)
                            {
                                int rowCount = worksheet.Dimension.Rows;
                                for (int row = 2; row <= rowCount; row++)
                                {
                                    var description = worksheet.Cells[row, 1].Text;
                                    var code = worksheet.Cells[row, 2].Text;

                                    var product = new Product
                                    {
                                        Description = description,
                                        Code = code,
                                        Id = Guid.NewGuid().ToString()
                                    };

                                    _context.Products.Add(product);
                                }

                                await _context.SaveChangesAsync();
                                ViewBag.messageSucessfull = "Products has been successfully created.";
                            }
                            return View(model);
                        }

                    }
                }
                else
                {
                    ViewBag.messageError = "The file .xlsx is not valid.";
                    return View(model);
                }
            }
            return View(model);
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

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product model, IFormFile image)
        {
            if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrEmpty(model.Code))
            {
                if (image != null)
                {
                    using var ms = new MemoryStream();
                    await image.CopyToAsync(ms);
                    byte[] imageBytes = ms.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    // Guardar la cadena en el modelo
                    model.ImagePath = base64String;
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
            if (action == "check")
            {
                var products = _context.Products.ToList();
                var productUsed = products.Where(w => w.Code == model.Code && w.Description == model.Description).ToList();
                if (!string.IsNullOrEmpty(model.Code) && productUsed.Count > 0)
                {
                    ModelState.Clear();
                    var selectedProduct = _productServices.getRandomProduct(products, true);
                    ViewBag.messageSucessfull = "Great";
                    return View(selectedProduct);
                }
                else
                {
                    ViewBag.messageError = "The code is invalid. Please try again.";
                    return View(model);
                }
            }

            else
            {
                var products = _context.Products.ToList();
                ModelState.Clear();
                var selectedProduct = _productServices.getRandomProduct(products, true);
                ViewBag.messageSucessfull = "";
                return View(selectedProduct);

            }
        }
    }
}
