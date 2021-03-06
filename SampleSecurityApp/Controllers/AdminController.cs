﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SampleSecurityApp.Models;
using SampleSecurityApp.ViewModels;

namespace SampleSecurityApp.Controllers
{
    public class AdminController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<CustomIdentityUser> _userManager;
        private IEmailSender _emailSender;
        public AdminController(RoleManager<IdentityRole> roleManager,
            UserManager<CustomIdentityUser> userManager, IEmailSender emailSender)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        //
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

        [Authorize(Policy = "CreateUserPolicy")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "CreateUserPolicy")]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CustomIdentityUser newUser = new CustomIdentityUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FullName = model.FullName,
                        Address = model.Address,
                        City = model.City
                    };
                    string password = Guid.NewGuid().ToString().Substring(0, 8);

                    //kirim email
                    var result = await _userManager.CreateAsync(newUser, password);
                    if (result.Succeeded)
                    {
                        await _emailSender.SendEmailAsync(model.Email, "Username dan Password - Security App",
                            $"Username: {model.Email} dan Password: {password}, silahkan melakukan change password");
                        return RedirectToAction("ListUsers", "Admin");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View(model);
                }
            }


            return View(model);
        }


        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User dengan id {id} tidak ditemukan";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                City = user.City,
                Claims = userClaims.Select(c=>c.Type + " : " +c.Value).ToList(),
                Roles = userRoles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "CreateUserPolicy")]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    ViewBag.ErrorMessage = $"User dengan id {model.Id} tidak ditemukan";
                    return View("NotFound");
                }
                else
                {
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.FullName = model.FullName;
                    user.Address = model.Address;
                    user.City = model.City;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListUsers");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
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

            //mengirimkan info user yg terdaftar pada role
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

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

        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role dengan id: {roleId} tidak ditemukan";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                    userRoleViewModel.IsSelected = true;
                else
                    userRoleViewModel.IsSelected = false;

                model.Add(userRoleViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model,
            string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMEssage = $"Role with id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                //kalau dicentang dan user belum ada di dalam role maka tambahkan
                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < model.Count - 1)
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User dengan id {userId} tidak ditemukan";
                return View("NotFound");
            }

            var existingUserClaims = await _userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel
            {
                UserId = userId
            };
            foreach(var claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value=="true"))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User dengan id {model.UserId} tidak ditemukan";
                return View("NotFound");
            }

            //delete semua claims dari user
            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Tidak bisa mendelete semua claim dr user");
                return View(model);
            }

            //tambahkan claims yg dipilih

            //var selectClaimsLambda = 
            //model.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimType));
            var selectClaims = (from c in model.Claims
                               select new Claim(c.ClaimType,c.IsSelected?"true":"false")).ToList();

            result = await _userManager.AddClaimsAsync(user, selectClaims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Tidak dapat menambahkan claims ke user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });
        }

    }
}
