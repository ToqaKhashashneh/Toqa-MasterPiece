using Nestify.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Nestify.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
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

        //..........................Register ..........................//
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
                Role = "User",
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

        //..........................Login..........................//
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
                TempData["Error"] = "Invalid email or password.";
                return View(model);
            }
           //info about user (key,value)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName ?? "User"),
                new Claim(ClaimTypes.Role, user.Role)
            };
            
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            if (user.Role=="Admin")
                return RedirectToAction("Dashboard", "Admin");
            else
                return RedirectToAction("Profile", "User");

         
        }

        //..........................Logout..........................//
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }



        //..........................Change Password..........................//
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ProfileViewModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login");

            int userId = int.Parse(userIdClaim);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Profile");
            }

            var cp = model.ChangePassword;

            if (!BCrypt.Net.BCrypt.Verify(cp.CurrentPassword, user.PasswordHash))
            {
                TempData["Error"] = "Current password is incorrect.";
                return RedirectToAction("Profile");
            }

            if (cp.NewPassword != cp.ConfirmPassword)
            {
                TempData["Error"] = "New password and confirmation do not match.";
                return RedirectToAction("Profile");
            }

            if (!Regex.IsMatch(cp.NewPassword, @"^(?=.*[a-z])(?=.*[A-Z]).{8,}$"))
            {
                TempData["Error"] = "Password must be at least 8 characters long and include both uppercase and lowercase letters.";
                return RedirectToAction("Profile");
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(cp.NewPassword);
            _context.SaveChanges();

            TempData["Success"] = "Password updated successfully.";
            return RedirectToAction("Profile");
        }







        //..........................Profile..........................//
        [Authorize]
        public IActionResult Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login");

            int userId = int.Parse(userIdClaim);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login");

            var activePayment = _context.Payments
                .Where(p => p.UserId == userId && p.Status == "Completed" && p.PackageId != null)
                .OrderByDescending(p => p.PaymentDate)
                .FirstOrDefault();

            bool canAddProperty = false;
            if (activePayment != null)
            {
                var package = _context.Packages.FirstOrDefault(p => p.Id == activePayment.PackageId);
                if (package != null)
                {
                    var usedPosts = _context.Properties.Count(p => p.PaymentId == activePayment.Id);
                    var expiryDate = activePayment.PaymentDate.Value.AddDays(package.DurationInDays);

                    if (DateTime.Now <= expiryDate && usedPosts < package.PostLimit)
                    {
                        canAddProperty = true;
                        var usedFeatured = _context.Properties.Count(p => p.PaymentId == activePayment.Id && p.IsFeatured == true);
                        ViewBag.UsedFeaturedPosts = usedFeatured;
                        ViewBag.FeaturedPostLimit = package.FeaturedPostLimit;

             

              
                    }
                }
            }

            ViewBag.CanAddProperty = canAddProperty;
            ViewBag.UserId = user.Id;
            ViewBag.FullName = user.FirstName + " " + user.LastName;
            ViewBag.FirstName = user.FirstName;
            ViewBag.LastName = user.LastName;
            ViewBag.Email = user.Email;
            ViewBag.PhoneNumber = user.PhoneNumber;
            ViewBag.Address = user.City + " " + user.Country;
            ViewBag.City = user.City;
            ViewBag.Country = user.Country;
            ViewBag.ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl) ? "/img/profile/DefaultUser.jpeg" : user.ProfileImageUrl;
            ViewBag.Locations = _context.Locations.ToList();
            ViewBag.SubLocations = _context.Sublocations.ToList();
          
            ViewBag.userProperties =  _context.Properties
            .Where(p => p.UserId == userId)
            .Include(p => p.SubLocation)
            .ToList();


            var favoriteProperties = _context.FavoriteProperties
            .Where(f => f.UserId == userId)
            .Include(f => f.Property)
                .ThenInclude(p => p.SubLocation)
            .Select(f => f.Property)
            .ToList();

             ViewBag.FavoriteProperties = favoriteProperties;


            var visitRequests = _context.PropertyInquiries
            .Where(i => i.UserId.HasValue && i.UserId.Value == userId)
            .Include(i => i.Property)
                .ThenInclude(p => p.SubLocation)
            .ToList();

            ViewBag.VisitRequests = visitRequests;


            return View();
        }


        //..........................Edit Profile..........................//
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(User user, IFormFile ProfileImageFile)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login");
            int userId = int.Parse(userIdClaim);

            var existingUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (existingUser == null) return RedirectToAction("Login");

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.City = user.City;
            existingUser.Country = user.Country;

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


        //..........................Remove From Favorite..........................//
        [HttpGet]
       public IActionResult RemoveFavorite(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login");
            int userId = int.Parse(userIdClaim);
            var favoriteProperty = _context.FavoriteProperties.FirstOrDefault(f => f.UserId == userId && f.PropertyId == id);
            if (favoriteProperty != null)
            {
                _context.FavoriteProperties.Remove(favoriteProperty);
                _context.SaveChanges();
                TempData["Success"] = "Property removed from favorites successfully!";

            }
            return RedirectToAction("Profile");
        }

        //..........................Load Sublocation..........................//

        [HttpGet]
        public JsonResult GetSublocationsByLocation(int locationId)
        {
            var sublocations = _context.Sublocations
                .Where(s => s.LocationId == locationId)
                .Select(s => new { s.Id, s.SublocationName })
                .ToList();

            return Json(sublocations);
        }




        //..........................Add Property..........................//

        [HttpPost]
        public IActionResult AddProperty(PropertyViewModel propertyViewModel, List<IFormFile> UploadedImages)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login");
            int userId = int.Parse(userIdClaim);

         
            
            var activePayment = _context.Payments
                .Where(p => p.UserId == userId && p.Status == "Completed" && p.PackageId != null)
                .OrderByDescending(p => p.PaymentDate)
                .FirstOrDefault();

            if (activePayment == null)
            {
                TempData["Error"] = "No active subscription found.";
                return RedirectToAction("Profile", "User");
            }

            var package = _context.Packages.FirstOrDefault(p => p.Id == activePayment.PackageId);
            if (package == null)
            {
                TempData["Error"] = "Invalid package associated with your payment.";
                return RedirectToAction("Profile", "User");
            }

            var usedPosts = _context.Properties.Count(p => p.PaymentId == activePayment.Id);
            if (usedPosts >= package.PostLimit || DateTime.Now > activePayment.PaymentDate.Value.AddDays(package.DurationInDays))
            {
                TempData["Error"] = "Your post limit is used up or plan expired.";
                return RedirectToAction("Profile", "User");
            }
            
            var property = new Property
            {
                PropertyName = propertyViewModel.PropertyName,
                UserId = userId,
                Bedrooms = propertyViewModel.Bedrooms,
                Bathrooms = propertyViewModel.Bathrooms,
                Size = propertyViewModel.Size,
                Price = propertyViewModel.Price,
                IsFeatured = propertyViewModel.IsFeatured,
                Description = propertyViewModel.Description,
                YearBuilt = propertyViewModel.YearBuilt,
                LotArea = propertyViewModel.LotArea,
                LotDimensions = propertyViewModel.LotDimensions,
                PropertyStatus = propertyViewModel.PropertyStatus,
                VideoUrl = propertyViewModel.VideoUrl,
                Latitude = propertyViewModel.Latitude,
                Longitude = propertyViewModel.Longitude,
                PublishDate = DateTime.Now,
                SubLocationId = propertyViewModel.SubLocationId,
                PropertyType = propertyViewModel.PropertyType,
                PriceType = propertyViewModel.PriceType,
                FacingDirection = propertyViewModel.FacingDirection,
                IsHidden = propertyViewModel.IsHidden,
                PaymentId = activePayment.Id
            };

            if (propertyViewModel.Features != null && propertyViewModel.Features.Any())
            {
                foreach (var f in propertyViewModel.Features)
                {
                    property.PropertyFeatures.Add(new PropertyFeature
                    {
                        FeatureName = f.FeatureName,
                        Size = f.Size
                    });
                }
            }

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



        //..........................Subscription(Payment)..........................//

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitSubscription(ProfileViewModel model)
        {
            var sub = model.Subscription;

            var user = _context.Users.FirstOrDefault(u => u.Id == sub.UserId);
            var package = _context.Packages.FirstOrDefault(p => p.Id == sub.PackageId);

            if (user == null || package == null)
            {
                TempData["Error"] = "Invalid user or package.";
                return RedirectToAction("Profile");
            }

            var isBusinessPlan = package.PostLimit >= 5;

            var payment = new Payment
            {
                UserId = sub.UserId,
                PackageId = sub.PackageId,
                Amount = package.Price,
                PaymentMethod = sub.PaymentMethod,
                PaymentDate = DateTime.Now,
                Status =  "Completed",

                IsBusinessPlan = isBusinessPlan
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            TempData["Success"] = "Subscription created successfully.";
            return RedirectToAction("Profile");
        }


        //..........................Edit Property..........................//
        [HttpGet]
        public async Task<IActionResult> EditProperty(int id)
        {
            var property = await _context.Properties
                .Include(p => p.PropertyFeatures)
                .Include(p => p.SubLocation)
                .ThenInclude(s => s.Location)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            var viewModel = new PropertyViewModel
            {
                Id = property.Id,
                PropertyName = property.PropertyName,
                Description = property.Description,
                Price = property.Price,
                PriceType = property.PriceType,
                PropertyType = property.PropertyType,
                PropertyStatus = property.PropertyStatus,
                SubLocationId = property.SubLocationId,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                Size = property.Size,
                LotArea = property.LotArea,
                LotDimensions = property.LotDimensions,
                FacingDirection = property.FacingDirection,
                YearBuilt = property.YearBuilt,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                LocationId = property.SubLocation?.LocationId ?? 0,
                ExistingImages = new List<string>
        {
            property.ImageUrl1,
            property.ImageUrl2,
            property.ImageUrl3,
            property.ImageUrl4,
            property.ImageUrl5
        }.Where(i => !string.IsNullOrEmpty(i)).ToList(),
                Features = property.PropertyFeatures.Select(f => new FeatureInputModel
                {
                    Id = f.Id,
                    FeatureName = f.FeatureName,
                    Size = f.Size
                }).ToList()
            };

            ViewBag.Locations = await _context.Locations.ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProperty(PropertyViewModel model)
        {
            var property = await _context.Properties
                .Include(p => p.PropertyFeatures)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (property == null) return NotFound();

            // Basic fields
            property.PropertyName = model.PropertyName;
            property.Description = model.Description;
            property.Price = model.Price;
            property.PriceType = model.PriceType;
            property.PropertyType = model.PropertyType;
            property.PropertyStatus = model.PropertyStatus;
            property.SubLocationId = model.SubLocationId;
            property.Latitude = model.Latitude;
            property.Longitude = model.Longitude;
            property.Size = model.Size;
            property.LotArea = model.LotArea;
            property.LotDimensions = model.LotDimensions;
            property.FacingDirection = model.FacingDirection;
            property.YearBuilt = model.YearBuilt;
            property.Bedrooms = model.Bedrooms;
            property.Bathrooms = model.Bathrooms;

            // Features
            _context.PropertyFeatures.RemoveRange(property.PropertyFeatures);
            property.PropertyFeatures = model.Features.Select(f => new PropertyFeature
            {
                FeatureName = f.FeatureName,
                Size = f.Size,
                PropertyId = model.Id
            }).ToList();

            // Images
            if (model.UploadedImages?.Any() == true)
            {
                var uploadsDir = Path.Combine("wwwroot", "uploads", "properties");
                Directory.CreateDirectory(uploadsDir);

                var imageUrls = new List<string>();

                foreach (var file in model.UploadedImages.Take(5))
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var path = Path.Combine(uploadsDir, fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    imageUrls.Add("/uploads/properties/" + fileName);
                }

                property.ImageUrl1 = imageUrls.ElementAtOrDefault(0);
                property.ImageUrl2 = imageUrls.ElementAtOrDefault(1);
                property.ImageUrl3 = imageUrls.ElementAtOrDefault(2);
                property.ImageUrl4 = imageUrls.ElementAtOrDefault(3);
                property.ImageUrl5 = imageUrls.ElementAtOrDefault(4);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Property updated successfully!";
            return RedirectToAction("Profile", "User");
        }




        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }




        // Delete Property
        [HttpPost]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await _context.Properties
                .Include(p => p.PropertyFeatures)
                .Include(p => p.PropertyInquiries)
                .Include(p => p.FavoriteProperties)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                TempData["Error"] = "Property not found.";
                return RedirectToAction("Profile");
            }

            // Remove related entities first
            _context.PropertyFeatures.RemoveRange(property.PropertyFeatures);
            _context.PropertyInquiries.RemoveRange(property.PropertyInquiries);
            _context.FavoriteProperties.RemoveRange(property.FavoriteProperties);

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Property has been deleted.";
         
            return RedirectToAction("Profile");
        }


    }
}
