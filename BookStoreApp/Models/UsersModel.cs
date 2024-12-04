using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class UsersModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
