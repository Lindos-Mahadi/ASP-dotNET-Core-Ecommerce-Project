using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingStore.Areas.Admin.Models;
using ShoppingStore.Areas.Admin.RoleModels;
using ShoppingStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _db;
        public RoleController(RoleManager<IdentityRole> roleManager, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _db = db;
            _userManager = userManager;
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

        // Get action Delete Method
        public async Task<IActionResult> Assign()
        {
            ViewData["UserId"] = new SelectList(_db.ApplicationUsers.Where(c => c.LockoutEnd < DateTime.Now || c.LockoutEnd == null).ToList(), "Id", "UserName");
            ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            return View();
        }

        // POST action Assign Method
        [HttpPost]
        public async Task<IActionResult> Assign(RoleUserViewModel roleUserViewModel)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == roleUserViewModel.UserId);
            // SAME USER ROLE ASSIGN CHECK
            var isCheckRoleAssign = await _userManager.IsInRoleAsync(user, roleUserViewModel.RoleId);
            if (isCheckRoleAssign)
            {
                ViewBag.msg = "This User is already Assigned";
                ViewData["UserId"] = new SelectList(_db.ApplicationUsers.Where(c => c.LockoutEnd < DateTime.Now || c.LockoutEnd == null).ToList(), "Id", "UserName");
                ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
                return View();
            }
            var role = await _userManager.AddToRoleAsync(user, roleUserViewModel.RoleId);

            if (role.Succeeded)
            {
                TempData["save"] = "User Role has been Assigned Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult AssignUserRole()
        {
            var result = from ur in _db.UserRoles
                         join r in _db.Roles on ur.RoleId equals r.Id
                         join a in _db.ApplicationUsers on ur.UserId equals a.Id
                         select new UserRoleMaping()
                         {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             UserName = a.UserName,
                             RoleName = r.Name
                         };
            ViewBag.UserRoles = result;


            return View();
        }
    }
}
