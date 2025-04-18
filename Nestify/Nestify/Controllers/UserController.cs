﻿using Nestify.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Nestify.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nestify.Controllers
{
    public class UserController : Controller
    {

        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UserController(MyDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //..............................................................New User...............................
        public IActionResult Register()
        {
          
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return View(model);
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = "User", // Default Nestify role
                Country = "",
                City = "",
                PhoneNumber = "",
                ProfileImageUrl = ""
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ViewBag.RegisterSuccess = "Welcome to Nestify! Your account was created successfully.";
            return View(); 
        }




        //...............................................................Login User...............................


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ViewBag.LoginError = "Invalid email or password.";
                return View(model);
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FirstName ?? "User"),
                    new Claim(ClaimTypes.Role, user.Role)
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);// identity (collection of claims)
            var principal = new ClaimsPrincipal(identity); // principal (user with claims)

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal); // sign in the user

            return RedirectToAction(user.Role == "Admin" ? "AdminPanel" : "Profile", "User");
        }

        //...............................................................Logout User...............................
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
            return RedirectToAction("Login", "User");
        }



        //.....................................................User Profile...............................

       
        [Authorize]
        public IActionResult Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login");

            int userId = int.Parse(userIdClaim);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login");

            ViewBag.FullName = user.FirstName + " " + user.LastName;
            ViewBag.FirstName = user.FirstName;
            ViewBag.LastName = user.LastName;
            ViewBag.Email = user.Email;
            ViewBag.PhoneNumber = user.PhoneNumber;
            ViewBag.Address = user.City + " " + user.Country;
            ViewBag.City = user.City;
            ViewBag.Country = user.Country;
            ViewBag.ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl) ?"/img/profile/DefaultUser.jpeg ": user.ProfileImageUrl;
            ViewBag.Locations = _context.Locations.ToList();
            ViewBag.SubLocations = _context.Sublocations.ToList();


            return View();
        }


        //.....................................................Edit User Profile...............................

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(User user, IFormFile ProfileImageFile)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login");
            int userId = int.Parse(userIdClaim);

            var existingUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (existingUser == null) return RedirectToAction("Login");

            // Update fields
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.City = user.City;
            existingUser.Country = user.Country;

            // ✅ Handle profile image upload
            if (ProfileImageFile != null && ProfileImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ProfileImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/profile", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImageFile.CopyToAsync(stream);
                }

                existingUser.ProfileImageUrl = "/img/profile/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Profile");
        }


        //.....................................................Get Locations and Sublocations...............................

        [HttpGet]
        public JsonResult GetSublocationsByLocation(int locationId)
        {
            var sublocations = _context.Sublocations
                .Where(s => s.LocationId == locationId)
                .Select(s => new { s.Id, s.SublocationName })
                .ToList();

            return Json(sublocations);
        }

  




        //.....................................................Login Admin...............................
        [Authorize(Roles = "Admin")]
        public IActionResult AdminPanel()
        {
                return View();

        }


        //.....................................................Add Property...............................
        [HttpPost]
        public IActionResult AddProperty(Property property, List<IFormFile> UploadedImages)
        {



            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                TempData["Error"] = "You must be logged in to add a property.";
                return RedirectToAction("Login", "User");
            }

            int userId = int.Parse(userIdClaim);
            property.UserId = userId;
            property.PublishDate = DateTime.Now;

            var imagePaths = new List<string>();
            if (UploadedImages != null && UploadedImages.Count > 0)
            {
                
                foreach (var file in UploadedImages)
                {
                
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var path = Path.Combine(_env.WebRootPath, "uploads", "properties", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    imagePaths.Add("/uploads/properties/" + fileName);
                }

                property.ImageUrl1 = imagePaths.ElementAtOrDefault(0);
                property.ImageUrl2 = imagePaths.ElementAtOrDefault(1);
                property.ImageUrl3 = imagePaths.ElementAtOrDefault(2);
                property.ImageUrl4 = imagePaths.ElementAtOrDefault(3);
                property.ImageUrl5 = imagePaths.ElementAtOrDefault(4);
            }

            _context.Properties.Add(property);
            _context.SaveChanges();

            TempData["Success"] = "Property added successfully!";
            return RedirectToAction("Profile", "User");
        }



    }
}
