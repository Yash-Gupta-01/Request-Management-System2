using System.ComponentModel.DataAnnotations;

namespace RMS.Models
{
    public class UserDetails
    {
        [Key]
        public string Username { get; set; } = null!; // Auto-generated primary key

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public string Role { get; set; } = "Guest"; // Default role is Guest
    }
}
