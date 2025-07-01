using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Nestify.Models.ViewModels
{

    public class ProfileViewModel
    {
        public User? User { get; set; }
        public PropertyViewModel propertyViewModel { get; set; } = new PropertyViewModel();



        public ChangePasswordViewModel ChangePassword { get; set; } = new ChangePasswordViewModel();

        public SubscriptionInputModel Subscription { get; set; } = new SubscriptionInputModel();

        public List<Property>? MyProperties { get; set; }
        public bool CanAddProperty { get; set; }
        public List<Property>? FavoriteProperties { get; set; }

    }
    public class PropertyViewModel
    {
        public int Id { get; set; } = 0; // 0 for new property, >0 for existing property
        [Required(ErrorMessage = "Property name is required.")]
        public string PropertyName { get; set; } = null!;

        public int UserId { get; set; } 
        [Required(ErrorMessage = "Number of bedrooms is required.")]
        [Range(1, 10, ErrorMessage = "Bedrooms must be between 1 and 10.")]
        public int Bedrooms { get; set; }

        [Required(ErrorMessage = "Number of bathrooms is required.")]
        [Range(1, 10, ErrorMessage = "Bathrooms must be between 1 and 10.")]
        public int Bathrooms { get; set; }

        [Required(ErrorMessage = "Size is required.")]
        [Range(1, 100000, ErrorMessage = "Size must be a positive number.")]
        public decimal Size { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, 100000000, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        public bool IsFeatured { get; set; }

        public string? Description { get; set; }

        [Range(1800,2025, ErrorMessage = "Please enter a valid year.")]
        public int? YearBuilt { get; set; }

        [Range(0, 100000, ErrorMessage = "Lot area must be a valid number.")]
        public decimal? LotArea { get; set; }

        public string? LotDimensions { get; set; }

        [Required(ErrorMessage = "Property status is required.")]
        public string PropertyStatus { get; set; } = null!;

        public string? VideoUrl { get; set; }

        public string? ImageUrl1 { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }
        public string? ImageUrl4 { get; set; }
        public string? ImageUrl5 { get; set; }

        [Required(ErrorMessage = "Latitude is required.")]
        public decimal? Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required.")]
        public decimal? Longitude { get; set; }

        public DateTime? PublishDate { get; set; }

        [Required(ErrorMessage = "Sub-location is required.")]
        public int SubLocationId { get; set; }

        [Required(ErrorMessage = "Property type is required.")]
        public string PropertyType { get; set; } = null!;

        [Required(ErrorMessage = "Price type is required.")]
        public string PriceType { get; set; } = null!;

        [Required(ErrorMessage = "Facing direction is required.")]
        public string FacingDirection { get; set; } = null!;

        public bool IsHidden { get; set; }

        public int? PaymentId { get; set; }

        [Required]
        public int LocationId { get; set; }

        public List<FeatureInputModel> Features { get; set; } = new List<FeatureInputModel>();

        public List<string> ExistingImages { get; set; } = new List<string>();
       

        // For file upload
        public IFormFileCollection UploadedImages { get; set; }
        public List<string> ImagesToDelete { get; set; } = new List<string>();

    }


    public class FeatureInputModel
    {

        public int Id { get; set; } = 0; // 0 for new feature, >0 for existing feature

        public int PropertyId { get; set; } // Will be set in controller from session

        [Required(ErrorMessage = "Feature name is required.")]
        [StringLength(100, ErrorMessage = "Feature name can't exceed 100 characters.")]
        public string FeatureName { get; set; } = null!;

        //[RegularExpression(@"^\d+(\.\d+)?x\d+(\.\d+)?$", ErrorMessage = "Size must be in format like 5x7 or 3.5x4.")]

        public string? Size { get; set; }
    }




    public class SubscriptionInputModel
    {
        public int UserId { get; set; }

        [Required]
        public int PackageId { get; set; }

        [Required]

        public string PaymentMethod { get; set; } = string.Empty;

        public string? BusinessName { get; set; }
        public string? BusinessAddress { get; set; }

        public string? CardHolderName { get; set; }
        [Required]
        [StringLength(16, MinimumLength = 13, ErrorMessage = "Card number must be between 13 and 16 digits.")]
        public string? CardNumber { get; set; }
        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])/[0-9]{2}$", ErrorMessage = "Expiry date must be in MM/YY format.")]
        public string? ExpiryDate { get; set; }
        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV must be 3 or 4 digits.")]
        public string? CVV { get; set; }
    }



    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string NewPassword { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }








}
