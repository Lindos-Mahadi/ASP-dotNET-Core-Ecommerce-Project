using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Data;
using ShoppingStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserController : Controller
    {
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _db;
        public UserController(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        // View User List
        public IActionResult Index()
        {
            return View(_db.ApplicationUsers.ToList());
        }
        // Create User
        public async Task<IActionResult> Create()
        { 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, user.PasswordHash);
                if (result.Succeeded)
                {
                    //var isSaveRole = await _userManager.AddToRoleAsync(user, role: "User");
                    TempData["save"] = "User has been Created Successfully";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        // Edit User
        public async Task<IActionResult> Edit(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            var userInfo = _db.ApplicationUsers.FirstOrDefault(u => u.Id == user.Id);
            if (userInfo == null)
            {
                return NotFound();
            }
            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            var result = await _userManager.UpdateAsync(userInfo);
            if (result.Succeeded)
            {
                TempData["save"] = "User has been Updated Successfully";
                //ViewBag.save = "Register has been completed Successfully";
                return RedirectToAction("Index");
            }
            return View(userInfo);
        }
    }
}
