using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.Roles = roles;
            return View();
        }

        // Get action Create Method
        public IActionResult Create()
        {
            return View();
        }

        // POST action Create Method
        [HttpPost]
        public async Task <IActionResult> Create(string name)
        {

            // SAME ROLE NAME VALIDATION CHECK
            IdentityRole role = new IdentityRole();
            role.Name = name;
            var isExist = await _roleManager.RoleExistsAsync(role.Name);

            if (isExist)
            {
                ViewBag.msg = "This Role is already exist";
                ViewBag.name = name;
                return View();
            }
            // END OF SAME ROLE NAME VALIDATION CHECK

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                TempData["save"] = "Role has been saved Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        // Get action Edit Method
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.id = role.Id;
            ViewBag.name = role.Name;
            return View();
        }

        // Edit action Create Method
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string name)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            role.Name = name;
            var isExist = await _roleManager.RoleExistsAsync(role.Name);
            if (isExist)
            {
                ViewBag.msg = "This Role is already exist";
                ViewBag.name = name;
                return View();
            }
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData["save"] = "Role has been Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        // Get action Delete Method
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.id = role.Id;
            ViewBag.name = role.Name;
            return View();
        }

        // Edit action Delete Method
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["delete"] = "Role has been Deleted Successfully";
                return RedirectToAction("Index");
            }
            return View(role);
        }

    }
}
