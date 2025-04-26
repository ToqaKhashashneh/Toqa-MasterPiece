using System.ComponentModel.DataAnnotations;

namespace Nestify.Models.ViewModels
{
    public class ContactFormViewModel
    {

        [Required]
        public required string Name { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        public required string Message { get; set; }





    }
}
