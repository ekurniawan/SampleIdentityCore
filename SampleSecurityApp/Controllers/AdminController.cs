using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SampleSecurityApp.Models;
using SampleSecurityApp.ViewModels;

namespace SampleSecurityApp.Controllers
{
    public class AdminController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<CustomIdentityUser> _userManager;
        public AdminController(RoleManager<IdentityRole> roleManager,
            UserManager<CustomIdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole myRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                var result = await _roleManager.CreateAsync(myRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        //menampilkan semua role
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles; 
            return View(roles);
        }

        //menampilkan semua user yang ada
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }

        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role dengan id: {id} tidak ditemukan";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = id.ToString(),
                RoleName = role.Name
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            return View(model);
        }
    }
}
