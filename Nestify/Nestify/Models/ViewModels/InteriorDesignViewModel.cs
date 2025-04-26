using System.ComponentModel.DataAnnotations;

namespace Nestify.Models.ViewModels
{
    public class InteriorDesignViewModel
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string Message { get; set; } = null!;

        public DateTime? SubmittedAt { get; set; }





    }
}
