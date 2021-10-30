using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingStore.Data;
using ShoppingStore.Models;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductTypesController : Controller
    {
        private ApplicationDbContext _db = null;
        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            //var data = _db.productTypes.ToList();
            return View(_db.productTypes.ToList());
        }
        // Create Get Action Method
        public ActionResult Create()
        {
            return View();
        }
        // Create Post Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _db.productTypes.Add(productTypes);
                await _db.SaveChangesAsync();
                TempData["save"] = "Data has been Saved Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }

        // Edit Get Action Method
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productType = _db.productTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }
        // Edit Post Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                _db.Update(productTypes);
                await _db.SaveChangesAsync();
                TempData["edit"] = "Data has been updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }
        // Details Get Action Method
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productType = _db.productTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }
        // Details Post Action Method
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Details(ProductTypes productTypes)
        //{
        //    return RedirectToAction(nameof(Index));
        //}
        // Delete Get Action Method
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productType = _db.productTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        // Edit Post Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, ProductTypes productTypes)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id != productTypes.Id)
            {
                return NotFound();
            }
            var productType = _db.productTypes.Find(id);
            if (productType == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _db.Remove(productType);
                await _db.SaveChangesAsync();
                TempData["delete"] = "Data has been Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }

    }
}
