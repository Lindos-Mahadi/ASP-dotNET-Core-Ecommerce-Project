using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Data;
using ShoppingStore.Models;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext _db = null;
        // for image 
        private IHostingEnvironment _he;
        public ProductsController(ApplicationDbContext db, IHostingEnvironment he)
        {
            _db = db;
            _he = he;
        }
        public IActionResult Index()
        {
            return View(_db.Products
                .Include(pt => pt.ProductTypes)
                .Include(st => st.SpecialTag)
                .ToList()
            );
        }
        // Post Index Action Method
        [HttpPost]
        public IActionResult Index(decimal? lowPrice, decimal? largePrice)
        {
            var products = _db.Products.Include(pt => pt.ProductTypes).Include(st => st.SpecialTag)
                .Where(c => c.Price >= lowPrice && c.Price <= largePrice).ToList();
            if (largePrice == null || largePrice == null)
            {
                //products = _db.Products.Include(pt => pt.ProductTypes).Include(st => st.SpecialTag).ToList();
                return View(_db.Products.Include(pt => pt.ProductTypes).Include(st => st.SpecialTag).ToList());
            }
            return View(products);
        }
        // Create Get Action Method
        public ActionResult Create()
        {
            ViewData["ProductTypeId"] = new SelectList(_db.productTypes.ToList(), "Id", "ProductType");
            ViewData["TagId"] = new SelectList(_db.specialTags.ToList(), "Id", "SpecialTagName");
            return View();
        }
        // Create Post Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Products products, IFormFile image)
        {

            if (ModelState.IsValid)
            {
                var searchProducts = _db.Products.FirstOrDefault(c => c.Name == products.Name);
                if (searchProducts != null)
                {
                    ViewData["ProductTypeId"] = new SelectList(_db.productTypes.ToList(), "Id", "ProductType");
                    ViewData["TagId"] = new SelectList(_db.specialTags.ToList(), "Id", "SpecialTagName");
                    ViewBag.messahge = "This Name is already exist";
                    return View(products);
                }
                if (image != null)
                {
                    // images location after "/"
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.Image = "Images/" + image.FileName;
                }
                if (image == null)
                {
                    products.Image = "Images/noimage.png";
                }

                _db.Products.Add(products);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // GET Edit Method
        public ActionResult Edit(int? id)
        {

            ViewData["ProductTypeId"] = new SelectList(_db.productTypes.ToList(), "Id", "ProductType");
            ViewData["TagId"] = new SelectList(_db.specialTags.ToList(), "Id", "SpecialTagName");
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products
                .Include(pt => pt.ProductTypes)
                .Include(st => st.SpecialTag)
                .FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // POST Edit Method
        [HttpPost]
        public async Task<IActionResult> Edit(Products products, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var searchProducts = _db.Products.FirstOrDefault(c => c.Name == products.Name);
                if (searchProducts != null)
                {
                    ViewData["ProductTypeId"] = new SelectList(_db.productTypes.ToList(), "Id", "ProductType");
                    ViewData["TagId"] = new SelectList(_db.specialTags.ToList(), "Id", "SpecialTagName");
                    ViewBag.messahge = "This Name is already exist";
                    return View(products);
                }
                if (image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    products.Image = "Images/" + image.FileName;
                }
                if (image == null)
                {
                    products.Image = "Images/noimage.png";
                }

                _db.Products.Update(products);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }
        // GET Details Method
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products
                .Include(pt => pt.ProductTypes)
                .Include(st => st.SpecialTag)
                .FirstOrDefault(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // GET Delete Method
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products            
                .Include(pt => pt.ProductTypes)
                .Include(st => st.SpecialTag)
                .Where(c => c.Id == id)
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // GET Delete Method
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _db.Products.FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
