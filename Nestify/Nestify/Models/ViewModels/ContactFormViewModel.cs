using System.ComponentModel.DataAnnotations;

namespace Nestify.Models.ViewModels
{
    public class ContactFormViewModel
    {

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        public string Message { get; set; }





    }
}
