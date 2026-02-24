using System.ComponentModel.DataAnnotations;
using AutoFleet.Core.Enums;

namespace AutoFleet.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        // Store hash, not pass
        public string PasswordHash { get; set; } = string.Empty; 
        
        public UserRole Role { get; set; } = UserRole.ADMIN;
    }
}
