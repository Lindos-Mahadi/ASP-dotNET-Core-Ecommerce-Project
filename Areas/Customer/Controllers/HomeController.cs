using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingStore.Data;
using ShoppingStore.Models;
using ShoppingStore.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace ShoppingStore.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        //public IActionResult Index()
        //{
        //    return View(_db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag).ToList());
        //}

        //PAGINATION
        public IActionResult Index(int? page)
        {
            return View(_db.Products.Include(c => c.ProductTypes).Include(c => c.SpecialTag).ToList().ToPagedList(pageNumber: page ?? 1, pageSize: 8));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // Get Details Method
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Get Details Method
        [HttpPost]
        [ActionName("Details")]
        public ActionResult ProductDetails(int? id)
        {
            List<Products> addProducts = new List<Products>();
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(c => c.ProductTypes).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            addProducts = HttpContext.Session.Get<List<Products>>("products");
            if (addProducts == null)
            {
                addProducts = new List<Products>();
            }
            addProducts.Add(product);
            HttpContext.Session.Set("products", addProducts);

            return View(product);
        }
        // Remove Add To Cart GET method
        [ActionName("Remove")]
        public IActionResult RemoveToCart(int? id)
        {

            List<Products> addProducts = HttpContext.Session.Get<List<Products>>("products");
            if (addProducts != null)
            {
                var product = addProducts.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    addProducts.Remove(product);
                    HttpContext.Session.Set("products", addProducts);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // Remove POST action method
        [HttpPost]
        public IActionResult Remove(int? id)
        {

            List<Products> addProducts = HttpContext.Session.Get<List<Products>>("products");
            if (addProducts != null)
            {
                var product = addProducts.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    addProducts.Remove(product);
                    HttpContext.Session.Set("products", addProducts);
                }
            }
            return RedirectToAction(nameof(Index));
        }
        // GET product Cart action Method
        public IActionResult CartView()
        {
            List<Products> addProducts = HttpContext.Session.Get<List<Products>>("products");
            if (addProducts == null)
            {
                addProducts = new List<Products>();
            }
            return View(addProducts);
        }
    }
}
