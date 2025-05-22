using System.ComponentModel.DataAnnotations;

namespace FamilyPhotos.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "A két jelszó nem egyezik.")]
        public string ConfirmPassword { get; set; }

    }
}
