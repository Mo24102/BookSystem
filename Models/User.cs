using System.ComponentModel.DataAnnotations;

namespace Booking_System.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Client> Clients { get; set; } = new List<Client>();
    }
}
