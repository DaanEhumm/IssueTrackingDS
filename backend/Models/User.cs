using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTrackingDS.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string? PasswordHash { get; set; }

        [Required]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; }

        // Navigatie property: een gebruiker kan meerdere tickets aanmaken
        public ICollection<Ticket>? CreatedTickets { get; set; }
        public ICollection<Ticket>? AssignedTickets { get; set; }
    }

    public enum UserRole
    {
        user,
        admin
    }
}