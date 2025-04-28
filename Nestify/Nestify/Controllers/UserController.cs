using Nestify.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Nestify.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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

        //..........................Register and Login..........................//
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

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            if (user.Role=="Admin")
                return RedirectToAction("Dashboard", "Admin");
            else
                return RedirectToAction("Profile", "User");

            //return RedirectToAction(user.Role == "Admin" ? "Dashboard" : "Profile", "User");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }









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

            //var viewModel = new ProfileViewModel
            //{
            //    User = user,
            //    Subscription = new SubscriptionInputModel(),
            //    propertyViewModel = new PropertyViewModel(),
            //    CanAddProperty = canAddProperty,
            //    MyProperties = userProperties
            //};
            return View();
        }

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

            //if (!ModelState.IsValid)
            //{
            //    TempData["Error"] = "All fields are required!";
            //    return RedirectToAction("Profile", "User");
            //}

            
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
              
                Features = property.PropertyFeatures.Select(f => new FeatureInputModel
                {
                    Id = f.Id,
                    FeatureName = f.FeatureName,
                    Size = f.Size
                }).ToList(),
            };

            ViewBag.Locations = await _context.Locations.ToListAsync();
            return View(viewModel);
        }
        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProperty(int id, PropertyViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // If invalid, collect errors and show in TempData
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = string.Join("\n", errors);
                ViewBag.Locations = await _context.Locations.ToListAsync();
                return View(model);
            }

            var property = await _context.Properties
                .Include(p => p.PropertyFeatures)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            // Update basic property fields
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
            property.IsHidden = model.IsHidden;
            property.PublishDate = DateTime.Now;

            // ------------------------
            // Handle Property Features
            // ------------------------
            // Clear old features
            _context.PropertyFeatures.RemoveRange(property.PropertyFeatures);

            // Add updated features
            if (model.Features != null && model.Features.Any())
            {
                property.PropertyFeatures = model.Features.Select(f => new PropertyFeature
                {
                    FeatureName = f.FeatureName,
                    Size = f.Size,
                    PropertyId = property.Id
                }).ToList();
            }
            else
            {
                property.PropertyFeatures = new List<PropertyFeature>();
            }

            // ------------------------
            // Handle Images Upload
            // ------------------------
            if (model.UploadedImages != null && model.UploadedImages.Count > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/properties");

                // Optional: delete old images if you want (depends on your logic)

                var savedImages = new List<string>();

                foreach (var file in model.UploadedImages.Take(5))
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadPath, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    savedImages.Add("/uploads/properties/" + uniqueFileName);
                }

                // Update image fields
                property.ImageUrl1 = savedImages.ElementAtOrDefault(0);
                property.ImageUrl2 = savedImages.ElementAtOrDefault(1);
                property.ImageUrl3 = savedImages.ElementAtOrDefault(2);
                property.ImageUrl4 = savedImages.ElementAtOrDefault(3);
                property.ImageUrl5 = savedImages.ElementAtOrDefault(4);
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Property updated successfully!";
                return RedirectToAction("Profile", "User");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating the property: " + ex.Message;
                ViewBag.Locations = await _context.Locations.ToListAsync();
                return View(model);
            }
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }

    }
}
